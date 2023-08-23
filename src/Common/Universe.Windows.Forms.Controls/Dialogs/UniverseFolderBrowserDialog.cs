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
using System.Windows.Forms;

namespace Universe.Windows.Forms.Controls.Dialogs
{
    /// <summary>
    ///    Предлагает пользователю выбрать папку.
    ///    Этот класс не наследуется.
    /// <author>Alex Envision</author>
    /// </summary>
    public sealed class UniverseFolderBrowserDialog : IDisposable
    {
        private string _selectedPath;

        private readonly FolderBrowserDialog _dialog;

        /// <summary>
        ///   Инициализирует новый экземпляр класса <see cref="T:FolderBrowserDialog" />.
        /// </summary>
        public UniverseFolderBrowserDialog()
        {
            _dialog = new FolderBrowserDialog();
        }

        /// <summary>
        ///   Инициализирует новый экземпляр класса <see cref="T:FolderBrowserDialog" />.
        /// </summary>
        public UniverseFolderBrowserDialog(string selectedPath)
        {
            _selectedPath = selectedPath;
            _dialog = new FolderBrowserDialog();
        }

        /// <summary>
        ///   Возвращает или задает значение, указывающее, является ли новую папку кнопка появляется в диалоговом окне браузера папки.
        /// </summary>
        /// <returns>
        ///   <see langword="true" /> Если новую папку кнопки отображаются в диалоговом окне, в противном случае — <see langword="false" />.
        ///    Значение по умолчанию — <see langword="true" />.
        /// </returns>
        public bool ShowNewFolderButton
        {
            get => this._dialog.ShowNewFolderButton;
            set => this._dialog.ShowNewFolderButton = value;
        }

        /// <summary>
        ///   Возвращает или задает путь, выбранный пользователем.
        /// </summary>
        /// <returns>
        ///   Путь папки, впервые выбранной в диалоговом окне, или последней папки, выбранной пользователем.
        ///    Значение по умолчанию — пустая строка ("").
        /// </returns>
        public string SelectedPath
        {
            get => this._selectedPath;
            set => this._selectedPath = value ?? string.Empty;
        }

        /// <summary>
        ///   Возвращает или задает корневую папку, где начинается просмотр.
        /// </summary>
        /// <returns>
        ///   Одно из значений <see cref="T:System.Environment.SpecialFolder" />.
        ///    Значение по умолчанию — <see langword="Desktop" />.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">
        ///   Присвоенное значение не является одним из <see cref="T:System.Environment.SpecialFolder" /> значения.
        /// </exception>
        public Environment.SpecialFolder RootFolder
        {
            get => this._dialog.RootFolder;
            set => this._dialog.RootFolder = value;
        }

        /// <summary>
        ///   Возвращает или задает описательный текст, отображаемый над элементом управления представления дерева в диалоговом окне.
        /// </summary>
        /// <returns>
        ///   Отображаемое описание.
        ///    Значение по умолчанию — пустая строка ("").
        /// </returns>
        public string Description
        {
            get => this._dialog.Description;
            set => this._dialog.Description = value ?? string.Empty;
        }

        private void ReInitialize()
        {
            _dialog.Description = @"Выберите папку содержащую файлы";
            _dialog.RootFolder = Environment.SpecialFolder.ProgramFiles;
            _dialog.ShowNewFolderButton = false;

            if (Directory.Exists(_selectedPath))
            {
                string x = _selectedPath;

                //Use API Flag to set correct path, following tahter a catch all better to check
                //enum for full list
                RootSetter.SetRootFolder(_dialog, RootSetter.CsIdl.FlagDontVerify);

                _dialog.SelectedPath = x;

            }
            else
            {
                _dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                _dialog.SelectedPath = "";
            }
        }

        public DialogResult ShowDialog()
        {
            ReInitialize();
            return _dialog.ShowDialog((IWin32Window)null);
        }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            ReInitialize();
            return _dialog.ShowDialog(owner);
        }

        public void Dispose()
        {
            _dialog?.Dispose();
        }
    }
}