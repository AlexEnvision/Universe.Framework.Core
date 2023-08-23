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
using System.Reflection;
using System.Windows.Forms;

namespace Universe.Windows.Forms.Controls.Dialogs
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    internal class RootSetter
    {
        [Flags]
        public enum CsIdl
        {
            Desktop = 0x0000, // Desktop
            Internet = 0x0001, // Internet Explorer (icon on desktop)
            Programs = 0x0002, // Start Menu\Programs
            Controls = 0x0003, // My Computer\Control Panel
            Printers = 0x0004, // My Computer\Printers
            Personal = 0x0005, // My Documents
            Favorites = 0x0006, // user name\Favorites
            Startup = 0x0007, // Start Menu\Programs\Startup
            Recent = 0x0008, // user name\Recent
            SendTo = 0x0009, // user name\SendTo
            BitBucket = 0x000a, // desktop\Recycle Bin
            StartMenu = 0x000b, // user name\Start Menu
            MyDocuments = 0x000c, // logical "My Documents" desktop icon
            MyMusic = 0x000d, // "My Music" folder
            MyVideo = 0x000e, // "My Videos" folder
            DesktopDirectory = 0x0010, // user name\Desktop
            Drives = 0x0011, // My Computer
            Network = 0x0012, // Network Neighborhood (My Network Places)
            Nethood = 0x0013, // user name\nethood
            Fonts = 0x0014, // windows\fonts
            Templates = 0x0015,
            CommonStartMenu = 0x0016, // All Users\Start Menu
            CommonPrograms = 0x0017, // All Users\Start Menu\Programs
            CommonStartup = 0x0018, // All Users\Startup
            CommonDesktopDirectory = 0x0019, // All Users\Desktop
            AppData = 0x001a, // user name\Application Data
            PrintHood = 0x001b, // user name\PrintHood
            LocalAppData = 0x001c, // user name\Local Settings\Applicaiton Data (non roaming)
            AltStartup = 0x001d, // non localized startup
            CommonAltStartup = 0x001e, // non localized common startup
            CommonFavorites = 0x001f,
            InternetCache = 0x0020,
            Cookies = 0x0021,
            History = 0x0022,
            CommonAppdata = 0x0023, // All Users\Application Data
            Windows = 0x0024, // GetWindowsDirectory()
            System = 0x0025, // GetSystemDirectory()
            ProgramFiles = 0x0026, // C:\Program Files
            MyPictures = 0x0027, // C:\Program Files\My Pictures
            Profile = 0x0028, // USERPROFILE
            SystemX86 = 0x0029, // x86 system directory on RISC
            ProgramFilesX86 = 0x002a, // x86 C:\Program Files on RISC
            ProgramFilesCommon = 0x002b, // C:\Program Files\Common
            ProgramFilesCommonx86 = 0x002c, // x86 Program Files\Common on RISC
            CommonTemplates = 0x002d, // All Users\Templates
            CommonDocuments = 0x002e, // All Users\Documents
            CommonAdminTools = 0x002f, // All Users\Start Menu\Programs\Administrative Tools
            AdminTools = 0x0030, // user name\Start Menu\Programs\Administrative Tools
            Connections = 0x0031, // Network and Dial-up Connections
            CommonMusic = 0x0035, // All Users\My Music
            CommonPictures = 0x0036, // All Users\My Pictures
            CommonVideo = 0x0037, // All Users\My Video
            Resources = 0x0038, // Resource Direcotry
            ResourcesLocalized = 0x0039, // Localized Resource Direcotry
            CommonOemLinks = 0x003a, // Links to All Users OEM specific apps
            CdBurnArea = 0x003b, // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
            ComputersNearMe = 0x003d, // Computers Near Me (computered from Workgroup membership)
            FlagCreate = 0x8000, // combine with CSIDL_ value to force folder creation in SHGetFolderPath()
            FlagDontVerify = 0x4000, // combine with CSIDL_ value to return an unverified folder path
            FlagNoAlias = 0x1000, // combine with CSIDL_ value to insure non-alias versions of the pidl
            FlagPerUserInit = 0x0800, // combine with CSIDL_ value to indicate per-user init (eg. upgrade)
            FlagMask = 0xFF00, // mask for all possible flag values
        }

        public static void SetRootFolder(FolderBrowserDialog fbd, CsIdl csidl)
        {
            Type t = fbd.GetType();
            FieldInfo fi = t.GetField("rootFolder", BindingFlags.Instance | BindingFlags.NonPublic);
            fi.SetValue(fbd, (System.Environment.SpecialFolder) csidl);
        }
    }
}