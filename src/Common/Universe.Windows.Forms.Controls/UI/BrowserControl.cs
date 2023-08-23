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

using System.IO;
using System.Linq;
using System.Windows.Forms;
using Universe.Windows.Forms.Controls.Extensions;
using Universe.Windows.Forms.Controls.UI.Events;
using Universe.Windows.Forms.Controls.UI.Events.EventArgs;

namespace Universe.Windows.Forms.Controls.UI
{
    public delegate void AfterSelectDirectoryDelegate(object sender, AfterSelectDirectoryEventArgs e);

    public delegate void CopyItemDelegate(object sender, CopyItemEventArgs e);

    public delegate void MoveItemDelegate(object sender, MoveItemEventArgs e);

    public delegate void RemoveItemDelegate(object sender, RemoveItemEventArgs e);

    public delegate void PasteItemDelegate(object sender, PasteItemEventArgs e);

    public delegate bool DirectoryExistsDelegate(object sender, DirectoryExistsEventArgs e);

    /// <summary>
    ///     Элемент управления для обзора файлов и папок, а также перемещения копирования и удаления.
    /// <author>Alex Envision</author>
    /// </summary>
    public partial class BrowserControl : UserControl
    {
        /// <summary>
        ///     Происходит, после выбора директории
        /// </summary>
        public event AfterSelectDirectoryDelegate AfterSelectDirectory;

        /// <summary>
        ///     Происходит, после выбора в контексном меню элемента управления пункта меню "Копировать"
        /// </summary>
        public event CopyItemDelegate CopyItem;

        /// <summary>
        ///     Происходит, после выбора в контексном меню элемента управления пункта меню "Переместить"
        /// </summary>
        public event MoveItemDelegate MoveItem;

        /// <summary>
        ///     Происходит, после выбора в контексном меню элемента управления пункта меню "Удалить"
        /// </summary>
        public event RemoveItemDelegate RemoveItem;

        /// <summary>
        ///     Происходит, после выбора в контексном меню элемента управления пункта меню "Вставить"
        /// </summary>
        public event PasteItemDelegate PasteItem;

        /// <summary>
        ///     Происходит, при проверке на существование директории
        /// </summary>
        public event DirectoryExistsDelegate DirectoryExists;

        /// <summary>
        ///     Происходит, когда нужно получить директории для отображения
        /// </summary>
        public event GetDirectoriesDelegate GetDirectories;

        /// <summary>
        ///     Происходит, когда нужно получить файлы для отображения
        /// </summary>
        public event GetFilesDelegate GetFiles;

        private string _rootFolder;

        public string RootFolder
        {
            get => _rootFolder;
            set
            {
                if (!DirectoryExists?.Invoke(this, new DirectoryExistsEventArgs{ CheckingDirectory = value}) ?? !Directory.Exists(value))
                    return;

                _rootFolder = value;
                tbStartFolder.Text = value;
                LoadTree(value);
                LoadFileList(value);
            }
        }

        public BrowserControl()
        {
            InitializeComponent();

            tvFolderBrowser.AfterSelect += treeFolderBrowser_AfterSelect;
            tvFolderBrowser.ApplyEvents();
        }

        public void ApplyEvents()
        {
            tvFolderBrowser.GetDirectories += GetDirectories;
            tvFolderBrowser.GetFiles += GetFiles;

            lvFileBrowser.GetDirectories += GetDirectories;
            lvFileBrowser.GetFiles += GetFiles;
        }

        public void UpdateState()
        {
            LoadTree(RootFolder);
            LoadFileList(RootFolder);
        }

        private void treeFolderBrowser_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var textNode = e.Node.FullPath;

            var rootFolder = RootFolder;
            var directoryName = rootFolder.GetParentFolder();
            if (string.IsNullOrEmpty(directoryName))
                directoryName = rootFolder;

            var fullPath = PathExtensions.Combine(directoryName, textNode);

            LoadFileList(fullPath);
        }

        private void LoadFileList(string fullPath)
        {
            lvFileBrowser.Clear();
            if (!DirectoryExists?.Invoke(this, new DirectoryExistsEventArgs { CheckingDirectory = fullPath }) ?? !Directory.Exists(fullPath))
                return;

            lvFileBrowser.ListFiles(fullPath);
        }

        private void tbSelectRootDirectory_Click(object sender, System.EventArgs e)
        {
            var selectedPath = string.Empty;
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    RootFolder = dialog.SelectedPath;
                }
                else
                {
                    return;
                }
            }

            LoadTree(RootFolder);

            AfterSelectDirectory?.Invoke(this, new AfterSelectDirectoryEventArgs {
                SelectedDirectory = selectedPath
            });
        }

        private void LoadTree(string rootFolder)
        {
            tvFolderBrowser.Clear();
            tvFolderBrowser.ListDirectory(rootFolder);
        }

        private void copyToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var selectedItems = this.lvFileBrowser.SelectedItems;
            if (selectedItems.Count == 0)
                return;

            var files = selectedItems.Select(x => x.Name).ToArray();

            CopyItem?.Invoke(this, new CopyItemEventArgs {
                Files = files
            });
        }

        private void moveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var selectedItems = this.lvFileBrowser.SelectedItems;
            if (selectedItems.Count == 0)
                return;

            var files = selectedItems.Select(x => x.Name).ToArray();

            MoveItem?.Invoke(this, new MoveItemEventArgs {
                Files = files
            });
        }

        private void removeItem_Click(object sender, System.EventArgs e)
        {
            var selectedItems = this.lvFileBrowser.SelectedItems;
            if (selectedItems.Count == 0)
                return;

            var files = selectedItems.Select(x => x.Name).ToArray();

            RemoveItem?.Invoke(this, new RemoveItemEventArgs
            {
                Files = files
            });
        }

        private void pasteToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var selectedNode = tvFolderBrowser.SelectedNode;

            var textNode = selectedNode != null 
                ? selectedNode.FullPath
                : lvFileBrowser.CurrentDirectory ?? string.Empty;

            //if (string.IsNullOrEmpty(textNode))
            //    return;

            var rootFolder = RootFolder;
            var directoryName = rootFolder.GetParentFolder();
            if (string.IsNullOrEmpty(directoryName))
                directoryName = rootFolder;

            var fullPath = PathExtensions.Combine(directoryName, textNode);

            PasteItem?.Invoke(this, new PasteItemEventArgs {
                DestinationFolder = fullPath
            });
        }
    }
}