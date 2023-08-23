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
using System.Threading.Tasks;
using System.Windows.Forms;
using Universe.Windows.Forms.Controls.Extensions;
using Universe.Windows.Forms.Controls.UI.Events;
using Universe.Windows.Forms.Controls.UI.Events.EventArgs;
using Universe.Windows.Forms.Controls.UI.Models;

namespace Universe.Windows.Forms.Controls.UI
{
    public partial class TreeFolderBrowserControl : UserControl
    {
        public TreeNode SelectedNode => tvMain?.SelectedNode;

        public event TreeViewEventHandler AfterSelect;

        /// <summary>
        ///     Происходит, когда нужно получить директории для отображения
        /// </summary>
        public event GetDirectoriesDelegate GetDirectories;

        /// <summary>
        ///     Происходит, когда нужно получить файлы для отображения
        /// </summary>
        public event GetFilesDelegate GetFiles;

        public TreeFolderBrowserControl()
        {
            InitializeComponent();

            this.FileFolder.Images.SetKeyName(0, "ic_folder_black_48dp_2x.png");
            this.FileFolder.Images.SetKeyName(1, "ic_description_black_48dp_2x.png");
        }

        public void ListDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!Directory.Exists(path))
                return;

            Parallel.Invoke(() => {
                if (GetDirectories != null && GetFiles != null)
                {
                    var node = CreateDirectoryNode(path);
                    this.tvMain.Nodes.Add(node);
                }
                else
                {
                    var rootDirectoryInfo = new DirectoryInfo(path);
                    this.tvMain.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
                }
            });
        }

        public void Clear()
        {
            this.tvMain.Nodes.Clear();
        }

        public TreeNode CreateDirectoryNode(string directoryInfo)
        {
            FolderFileNode directoryNode = new FolderFileNode(directoryInfo.GetFolderName(), directoryInfo, false);
            //foreach (var directory in GetDirectories?.Invoke(new GetDirectoriesEventArgs {Directory = directoryInfo}) ?? new ItemInfo[] { })
            //{
            //    try
            //    {
            //        directoryNode.Nodes.Add(CreateDirectoryNode(directory.FullName));
            //    }
            //    catch
            //    {
            //        // ignored
            //    }
            //}
            Parallel.ForEach(GetDirectories?.Invoke(new GetDirectoriesEventArgs { Directory = directoryInfo }) ?? new ItemInfo[] { }, (directory) => {
                lock (directoryNode)
                {
                    try
                    {
                        directoryNode.Nodes.Add(CreateDirectoryNode(directory.FullName));
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
            //        directoryNode.Nodes.Add(new FolderFileNode(file.Name, file.FullName, true));
            //    }
            //    catch
            //    {
            //        // ignored
            //    }
            //}
            Parallel.ForEach(GetFiles?.Invoke(new GetFilesEventArgs { Directory = directoryInfo }) ?? new ItemInfo[] { }, (file) =>
            {
                lock (directoryNode)
                {
                    try
                    {
                        directoryNode.Nodes.Add(new FolderFileNode(file.Name, file.FullName, true));
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });

            return directoryNode;
        }

        public static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            FolderFileNode directoryNode = new FolderFileNode(directoryInfo.Name, directoryInfo.FullName, false);
            Parallel.ForEach(directoryInfo.GetDirectories(), (directory) => {
                lock (directoryNode)
                {
                    try
                    {
                        directoryNode.Nodes.Add(CreateDirectoryNode(directory));
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });

            Parallel.ForEach(directoryInfo.GetFiles(), (file) =>
            {
                lock (directoryNode)
                {
                    try
                    {
                        directoryNode.Nodes.Add(new FolderFileNode(file.Name, file.FullName, true));
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });
            return directoryNode;
        }

        public void ApplyEvents()
        {
            this.tvMain.AfterSelect += AfterSelect;
        }
    }

    public class FolderFileNode : TreeNode
    {
        public string Path { get; set; }

        public FolderFileNode(string text, string path, bool isFile)
        {
            this.Text = text;
            this.Path = path;
            if (isFile)
            {
                this.ImageIndex = 1;
                this.SelectedImageIndex = 1;
            }
        }
    }
}