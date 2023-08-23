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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Universe.Windows.Forms.Controls.UI.Events;
using Universe.Windows.Forms.Controls.UI.Events.EventArgs;
using Universe.Windows.Forms.Controls.UI.Models;

namespace Universe.Windows.Forms.Controls.UI
{
    public partial class ListViewFileBrowserControl : UserControl
    {
        /// <summary>
        ///     Происходит, когда нужно получить директории для отображения
        /// </summary>
        public event GetDirectoriesDelegate GetDirectories;

        /// <summary>
        ///     Происходит, когда нужно получить файлы для отображения
        /// </summary>
        public event GetFilesDelegate GetFiles;

        public List<ListViewItem> SelectedItems => lvMain?.SelectedItems.Cast<ListViewItem>().ToList() ?? new List<ListViewItem>();

        public string CurrentDirectory { get; private set; }

        public ListViewFileBrowserControl()
        {
            InitializeComponent();

            this.FileFolder.Images.SetKeyName(0, "ic_folder_black_48dp_2x.png");
            this.FileFolder.Images.SetKeyName(1, "ic_description_black_48dp_2x.png");
        }

        public void ListFiles(string path)
        {
            path = path ?? string.Empty;
            CurrentDirectory = Path.GetFileName(path);

            Parallel.Invoke(() => {
                if (GetDirectories != null && GetFiles != null)
                {
                    lvMain.Items.AddRange(CreateFilesList(path));
                }
                else
                {
                    var rootDirectoryInfo = new DirectoryInfo(path);
                    lvMain.Items.AddRange(CreateFilesList(rootDirectoryInfo));
                }
            });
        }

        public ListViewItem[] CreateFilesList(string directoryInfo)
        {
            var lvItems = new List<ListViewItem>();
            //foreach (var directory in GetDirectories?.Invoke(new GetDirectoriesEventArgs {Directory = directoryInfo}) ?? new ItemInfo[] { })
            //{
            //    try
            //    {
            //        var lvItem = new ListViewItem
            //        {
            //            Text = directory.Name,
            //            Name = directory.FullName,
            //            ImageIndex = 0
            //        };
            //        lvItems.Add(lvItem);
            //    }
            //    catch
            //    {
            //        // ignored
            //    }
            //}
            Parallel.ForEach(GetDirectories?.Invoke(new GetDirectoriesEventArgs { Directory = directoryInfo }) ?? new ItemInfo[] { }, (directory) =>
            {
                lock (lvItems)
                {
                    try
                    {
                        var lvItem = new ListViewItem
                        {
                            Text = directory.Name,
                            Name = directory.FullName,
                            ImageIndex = 0
                        };
                        lvItems.Add(lvItem);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });

            //foreach (var file in GetFiles?.Invoke(new GetFilesEventArgs {Directory = directoryInfo}) ?? new ItemInfo[] { })
            //{
            //    try
            //    {
            //        var lvItem = new ListViewItem
            //        {
            //            Text = file.Name,
            //            Name = file.FullName,
            //            ImageIndex = 1
            //        };
            //        lvItems.Add(lvItem);
            //    }
            //    catch
            //    {
            //        // ignored
            //    }
            //}
            Parallel.ForEach(GetFiles?.Invoke(new GetFilesEventArgs { Directory = directoryInfo }) ?? new ItemInfo[] { }, (file) => {
                lock (lvItems)
                {
                    try
                    {
                        var lvItem = new ListViewItem
                        {
                            Text = file.Name,
                            Name = file.FullName,
                            ImageIndex = 1
                        };
                        lvItems.Add(lvItem);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });

            return lvItems.ToArray();
        }

        public static ListViewItem[] CreateFilesList(DirectoryInfo directoryInfo)
        {
            var lvItems = new List<ListViewItem>();
            Parallel.ForEach(directoryInfo.GetDirectories(), (directory) =>
            {
                lock (lvItems)
                {
                    try
                    {
                        var lvItem = new ListViewItem
                        {
                            Text = directory.Name,
                            Name = directory.FullName,
                            ImageIndex = 0
                        };
                        lvItems.Add(lvItem);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });

            Parallel.ForEach(directoryInfo.GetFiles(), (file) => {
                lock (lvItems)
                {
                    try
                    {
                        var lvItem = new ListViewItem
                        {
                            Text = file.Name,
                            Name = file.FullName,
                            ImageIndex = 1
                        };
                        lvItems.Add(lvItem);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });

            return lvItems.ToArray();
        }

        public void Clear()
        {
            this.lvMain.Items.Clear();
        }
    }
}