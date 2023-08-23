//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
//  ║                                                                                 ║
//  ║   Licensed under the Apache License, Version 2.0 (the "License");               ║
//  ║   you may not use this file except in compliance with the License.              ║
//  ║   You may obtain a copy of the License at                                       ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0                                ║
//  ║                                                                                 ║
//  ║   Unless required by applicable law or agreed to in writing, software           ║
//  ║   distributed under the License is distributed on an "AS IS" BASIS,             ║
//  ║   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.      ║
//  ║   See the License for the specific language governing permissions and           ║
//  ║   limitations under the License.                                                ║
//  ║                                                                                 ║
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
//  ║                                                                                 ║
//  ║   Лицензировано согласно Лицензии Apache, Версия 2.0 ("Лицензия");              ║
//  ║   вы можете использовать этот файл только в соответствии с Лицензией.           ║
//  ║   Вы можете найти копию Лицензии по адресу                                      ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0.                               ║
//  ║                                                                                 ║
//  ║   За исключением случаев, когда это регламентировано существующим               ║
//  ║   законодательством или если это не оговорено в письменном соглашении,          ║
//  ║   программное обеспечение распространяемое на условиях данной Лицензии,         ║
//  ║   предоставляется "КАК ЕСТЬ" и любые явные или неявные ГАРАНТИИ ОТВЕРГАЮТСЯ.    ║
//  ║   Информацию об основных правах и ограничениях,                                 ║
//  ║   применяемых к определенному языку согласно Лицензии,                          ║
//  ║   вы можете найти в данной Лицензии.                                            ║
//  ║                                                                                 ║
//  ╚═════════════════════════════════════════════════════════════════════════════════╝

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Universe.Diagnostic.Logger;
using Universe.Framework.FilesExplorer.Tests.Settings;
using Universe.Helpers.Extensions;
using Universe.IO.DownloadClient.Extensions;
using Universe.IO.DownloadClient.Interfaces;
using Universe.Windows.Forms.Controls;
using Universe.Windows.Forms.Controls.Extensions;
using Universe.Windows.Forms.Controls.Settings;
using Universe.Windows.Forms.Controls.UI.Events.EventArgs;
using Universe.Windows.Forms.Controls.UI.Models;

namespace Universe.Framework.FilesExplorer.Tests
{
    public partial class MainForm : Form
    {
        private EventLogger _log;

        private GeneralSettings _programSettings;

        private IDownloadClient _rootFolderSource;

        private IDownloadClient _rootFolderDestination;

        private readonly ExchangeBuffer _exchangeBuffer;

        public MainForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            _log = new EventLogger();

            _log.LogInfo += e => {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message = $"[{currentDate}] {e.Message}{Environment.NewLine}";
                    this.SafeCall(() => this.tbLog.AppendText(message));
                }
            };

            _log.LogError += e => {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message = $"[{currentDate}] Во время выполнения операции произошла ошибка. Текст ошибки: {e.Message}.{Environment.NewLine} Трассировка стека: {e.Ex.StackTrace}{Environment.NewLine}";
                    this.SafeCall(() => this.tbLog.AppendText(message));
                }
            };

            _exchangeBuffer = new ExchangeBuffer();

            browserControlSource.CopyItem += (sender, e) => {
                _exchangeBuffer.Files = e.Files;
                _exchangeBuffer.Type = ExchangeActionType.Copy;
            };

            browserControlSource.MoveItem += (sender, e) => {
                _exchangeBuffer.Files = e.Files;
                _exchangeBuffer.Type = ExchangeActionType.Move;
            };

            browserControlSource.RemoveItem += (sender, e) => {
                RemoveItems(_rootFolderSource, e.Files);
                browserControlSource.UpdateState();
            };

            browserControlSource.PasteItem += (sender, e) => {
                PasteItem(_rootFolderSource, e.DestinationFolder);
                browserControlSource.UpdateState();
            };

            browserControlDest.CopyItem += (sender, e) => {
                _exchangeBuffer.Files = e.Files;
                _exchangeBuffer.Type = ExchangeActionType.Copy;
            };

            browserControlDest.MoveItem += (sender, e) => {
                _exchangeBuffer.Files = e.Files;
                _exchangeBuffer.Type = ExchangeActionType.Move;
            };

            browserControlDest.RemoveItem += (sender, e) => {
                RemoveItems(_rootFolderDestination, e.Files);
                browserControlDest.UpdateState();
            };

            browserControlDest.PasteItem += (sender, e) =>
            {
                PasteItem(_rootFolderDestination, e.DestinationFolder);
                browserControlDest.UpdateState();
            };

            browserControlSource.AfterSelectDirectory += BrowserControlSourceOnAfterSelectDirectory;
            browserControlDest.AfterSelectDirectory += BrowserControlSourceOnAfterSelectDirectory;

            browserControlSource.GetDirectories += BrowserControlOnGetDirectories;
            browserControlSource.GetFiles += BrowserControlOnGetFiles;
            browserControlSource.ApplyEvents();

            browserControlDest.GetDirectories += BrowserControlOnGetDirectories;
            browserControlDest.GetFiles += BrowserControlOnGetFiles;
            browserControlDest.ApplyEvents();
        }

        private ItemInfo[] BrowserControlOnGetDirectories(GetDirectoriesEventArgs e)
        {
            if (cbAllowRunAsSystemUser.Checked)
            {
                return GetDirectoriesUnderAppUser(e);
            }
            else
            {
                return GetDirectoriesUnderAnotherUser(e);
            }
        }

        private ItemInfo[] GetDirectoriesUnderAnotherUser(GetDirectoriesEventArgs e)
        {
            var login = tbLogin.Text;
            if (login.IsNullOrEmpty())
            {
                _log.Info("Не указан логин пользователя!");
                return new ItemInfo[] { };
            }

            var password = tbPassword.Text;
            if (password.IsNullOrEmpty())
            {
                _log.Info("Не указан пароль пользователя!");
                return new ItemInfo[] { };
            }

            if (!Regex.IsMatch(login, @"^[a-zA-Z][a-zA-Z0-9\-\.]{0,61}[a-zA-Z]\\\w[\w\.\- ]+$"))
            {
                _log.Info(@"Логин указан в некорректном формате. Нужно указать в формате domain\username.");
                return new ItemInfo[] { };
            }

            try
            {
                var domain = login.Split('\\').FirstOrDefault();
                var username = login.Split('\\').LastOrDefault();

                var folderSettings = new FolderSettings
                {
                    Domain = domain,
                    UserName = username,
                    Password = password,
                    FolderUrl = new Uri(e.Directory)
                };

                var rootFolder = DownloadClientFactory.Create(folderSettings).GetRootFolder();
                var directories = rootFolder.GetFolders();

                return directories.Select(x => new ItemInfo
                {
                    Name = x.Name.GetFolderName(),
                    FullName = x.Uri.LocalPath
                }).ToArray();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return new ItemInfo[] { };
            }
        }

        private ItemInfo[] GetDirectoriesUnderAppUser(GetDirectoriesEventArgs e)
        {
            try
            {
                var folderSettings = new FolderSettings
                {
                    AllowRunUnderCurrentUser = true,
                    FolderUrl = new Uri(e.Directory)
                };

                var rootFolder = DownloadClientFactory.Create(folderSettings).GetRootFolder();
                var directories = rootFolder.GetFolders();

                return directories.Select(x => new ItemInfo
                {
                    Name = x.Name.GetFolderName(),
                    FullName = x.Uri.LocalPath
                }).ToArray();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return new ItemInfo[] { };
            }
        }

        private ItemInfo[] BrowserControlOnGetFiles(GetFilesEventArgs e)
        {
            if (cbAllowRunAsSystemUser.Checked)
            {
                return GetFilesUnderAppUser(e);
            }
            else
            {
                return GetFilesUnderAnotherUser(e);
            }
        }

        private ItemInfo[] GetFilesUnderAnotherUser(GetFilesEventArgs e)
        {
            var login = tbLogin.Text;
            if (login.IsNullOrEmpty())
            {
                _log.Info("Не указан логин пользователя!");
                return new ItemInfo[] { };
            }

            var password = tbPassword.Text;
            if (password.IsNullOrEmpty())
            {
                _log.Info("Не указан пароль пользователя!");
                return new ItemInfo[] { };
            }

            if (!Regex.IsMatch(login, @"^[a-zA-Z][a-zA-Z0-9\-\.]{0,61}[a-zA-Z]\\\w[\w\.\- ]+$"))
            {
                _log.Info(@"Логин указан в некорректном формате. Нужно указать в формате domain\username.");
                return new ItemInfo[] { };
            }

            try
            {
                var domain = login.Split('\\').FirstOrDefault();
                var username = login.Split('\\').LastOrDefault();

                var folderSettings = new FolderSettings
                {
                    Domain = domain,
                    UserName = username,
                    Password = password,
                    FolderUrl = new Uri(e.Directory)
                };

                var rootFolder = DownloadClientFactory.Create(folderSettings).GetRootFolder();
                var files = rootFolder.GetFiles();

                return files.Select(x => new ItemInfo
                {
                    Name = x.Name,
                    FullName = x.Uri.LocalPath
                }).ToArray();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return new ItemInfo[] { };
            }
        }

        private ItemInfo[] GetFilesUnderAppUser(GetFilesEventArgs e)
        {
            try
            {
                var folderSettings = new FolderSettings
                {
                    AllowRunUnderCurrentUser = true,
                    FolderUrl = new Uri(e.Directory)
                };

                var rootFolder = DownloadClientFactory.Create(folderSettings).GetRootFolder();
                var files = rootFolder.GetFiles();

                return files.Select(x => new ItemInfo
                {
                    Name = x.Name,
                    FullName = x.Uri.LocalPath
                }).ToArray();
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
                return new ItemInfo[] { };
            }
        }

        private void BrowserControlSourceOnAfterSelectDirectory(object sender, AfterSelectDirectoryEventArgs e)
        {
            OpenClients();
        }

        private void RemoveItems(IDownloadClient client, string[] elements)
        {
            foreach (var element in elements)
            {
                if (element == null)
                    continue;

                try
                {
                    var sourceFolderPath = element.IsDirectory() ? element : Path.GetDirectoryName(element);

                    var rootFolder = client.GetRootFolder();
                    var elementFolder = SearchFolder(rootFolder, sourceFolderPath);
                    if (elementFolder == null)
                    {
                        var elementFolderPath = element.IsDirectory() ? element : Path.GetDirectoryName(element) ?? string.Empty;

                        if (Path.GetFullPath(rootFolder.Uri.LocalPath) == Path.GetFullPath(elementFolderPath))
                        {
                            elementFolder = rootFolder;
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (element.IsDirectory())
                    {
                        elementFolder.RemoveFolder();
                        continue;
                    }

                    elementFolder.RemoveFile(element);
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message);
                }
            }
        }

        private void PasteItem(IDownloadClient client, string destinationFolder)
        {
            if (client == null)
                return;

            if (_exchangeBuffer.Files.Length == 0)
                return;

            try
            {
                var fstFilePath = _exchangeBuffer.Files.First();
                var sourceFolderPath = fstFilePath.IsDirectory() ? fstFilePath : Path.GetDirectoryName(fstFilePath);

                var rootFolder = client.GetRootFolder();
                var sourceFolder = SearchFolder(rootFolder, sourceFolderPath);

                var foundFolder = SearchFolder(rootFolder, destinationFolder);
                if (foundFolder == null)
                {
                    if (Path.GetFullPath(rootFolder.Uri.LocalPath) == Path.GetFullPath(destinationFolder))
                    {
                        foundFolder = rootFolder;
                    }
                    else
                    {
                        return;
                    }
                }

                foreach (var element in _exchangeBuffer.Files)
                {
                    if (element.IsDirectory())
                    {
                        var createdFolder = foundFolder.AddFolder(element);

                        if (_exchangeBuffer.Type == ExchangeActionType.Move)
                            sourceFolder.RemoveFolder();

                        continue;
                    }

                    var fileInfo = new FileInfo(element);
                    using (var fs = File.Open(element, FileMode.Open))
                    {
                        var buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);

                        foundFolder.AddFile(fileInfo.Name, buffer);
                        if (_exchangeBuffer.Type == ExchangeActionType.Move)
                            sourceFolder.RemoveFile(fileInfo.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
        }

        /// <summary>
        ///     Выполняет поиск папки
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        private IDownloadFolder SearchFolder(IDownloadFolder rootFolder, string folderName)
        {
            var folders = rootFolder.GetFolders();
            IDownloadFolder searchedFolder = null;
            foreach (var folder in folders)
            {
                if (folder.Name.PrepareToCompare() == folderName.PrepareToCompare())
                {
                    return folder;
                }

                searchedFolder = SearchFolder(folder, folderName);
            }

            return searchedFolder;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _programSettings = _programSettings.Load();
            LoadOnForm();
        }

        public void LoadOnForm()
        {
            tbLogin.Text = _programSettings.Login;
            tbPassword.Text = _programSettings.Password;
            cbAllowRunAsSystemUser.Checked = _programSettings.AllowImpersonated;

            browserControlSource.RootFolder = _programSettings.SourceStartFolder;
            browserControlDest.RootFolder = _programSettings.DestinationStartFolder;

            if (browserControlDest.RootFolder.IsNullOrEmpty())
                browserControlDest.RootFolder = browserControlSource.RootFolder;

            if (!browserControlSource.RootFolder.IsNullOrEmpty())
                OpenClients();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnLoadFromForm();
            _programSettings.Save();
        }

        public void UnLoadFromForm()
        {
            _programSettings.Login = tbLogin.Text;
            _programSettings.Password = tbPassword.Text;

            _programSettings.SourceStartFolder = browserControlSource.RootFolder;
            _programSettings.DestinationStartFolder = browserControlDest.RootFolder;

            _programSettings.AllowImpersonated = cbAllowRunAsSystemUser.Checked;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            if (cbAllowRunAsSystemUser.Checked)
            {
                OpenClientsUnderAppUser();
            }
            else
            {
                OpenClientsUnderAnotherUser();
            }
        }

        private void OpenClients()
        {
            if (cbAllowRunAsSystemUser.Checked)
            {
                OpenClientsUnderAppUser();
            }
            else
            {
                OpenClientsUnderAnotherUser();
            }
        }

        private void OpenClientsUnderAnotherUser()
        {
            var login = tbLogin.Text;
            if (login.IsNullOrEmpty())
            {
                _log.Info("Не указан логин пользователя!");
                return;
            }

            var password = tbPassword.Text;
            if (password.IsNullOrEmpty())
            {
                _log.Info("Не указан пароль пользователя!");
                return;
            }

            if (!Regex.IsMatch(login, @"^[a-zA-Z][a-zA-Z0-9\-\.]{0,61}[a-zA-Z]\\\w[\w\.\- ]+$"))
            {
                _log.Info(@"Логин указан в некорректном формате. Нужно указать в формате domain\username.");
                return;
            }

            try
            {
                var startFolderSource = browserControlSource.RootFolder;
                if (startFolderSource.IsNullOrEmpty())
                {
                    string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    _log.Info($"Не указана стартовая папка! Устанавливается {desktopFolder}.");
                    startFolderSource = desktopFolder;
                }

                var startFolderDestination = browserControlDest.RootFolder;
                if (startFolderDestination.IsNullOrEmpty())
                {
                    string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    _log.Info($"Не указана стартовая папка! Устанавливается {desktopFolder}.");
                    startFolderDestination = desktopFolder;
                }

                var domain = login.Split('\\').FirstOrDefault();
                var username = login.Split('\\').LastOrDefault();

                var folderSettingsSource = new FolderSettings
                {
                    Domain = domain,
                    UserName = username,
                    Password = password,
                    FolderUrl = new Uri(startFolderSource)
                };

                _rootFolderSource = DownloadClientFactory.Create(folderSettingsSource);

                var folderSettingsDestination = new FolderSettings
                {
                    Domain = domain,
                    UserName = username,
                    Password = password,
                    FolderUrl = new Uri(startFolderDestination)
                };

                _rootFolderDestination = DownloadClientFactory.Create(folderSettingsDestination);
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
        }

        private void OpenClientsUnderAppUser()
        {
            try
            {
                var startFolderSource = browserControlSource.RootFolder;
                if (startFolderSource.IsNullOrEmpty())
                {
                    string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    _log.Info($"Не указана стартовая папка! Устанавливается {desktopFolder}.");
                    startFolderSource = desktopFolder;
                }

                var startFolderDestination = browserControlDest.RootFolder;
                if (startFolderDestination.IsNullOrEmpty())
                {
                    string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    _log.Info($"Не указана стартовая папка! Устанавливается {desktopFolder}.");
                    startFolderDestination = desktopFolder;
                }

                var folderSettingsSource = new FolderSettings
                {
                    AllowRunUnderCurrentUser = true,
                    FolderUrl = new Uri(startFolderSource)
                };

                _rootFolderSource = DownloadClientFactory.Create(folderSettingsSource);

                var folderSettingsDestination = new FolderSettings
                {
                    AllowRunUnderCurrentUser = true,
                    FolderUrl = new Uri(startFolderDestination)
                };

                _rootFolderDestination = DownloadClientFactory.Create(folderSettingsDestination);
            }
            catch (Exception ex)
            {
                _log.Error(ex, ex.Message);
            }
        }
    }
}
