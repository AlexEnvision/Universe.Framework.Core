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
using System.Diagnostics;
using Universe.Diagnostic.Exceptions;
using Universe.Types;

namespace Universe.Diagnostic
{
    /// <summary>
    ///     Счётчик-наблюдатель времени выполнения
    /// <author>Alex Envision</author>
    /// </summary>
    public class RunningTimeWatcher : DisposableObject
    {
        private const string IsDisposedMessage = "Счетчик-наблюдатель времени выполнения был утилизирован и теперь недоступен. Проверьте код на наличие вызова Dispose().";

        private Stopwatch _stopwatch;

        private bool _isDisposed;

        public RunningTimeWatcher()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        /// <summary>
        ///     Сброс
        /// </summary>
        public void Reset()
        {
            if (_isDisposed)
                throw new RunningTimeWatcherException(IsDisposedMessage);

            _stopwatch.Reset();
        }

        /// <summary>
        ///     Получить время с начала запуска
        /// </summary>
        /// <returns></returns>
        public TimeSpan TakeRunningTime()
        {
            if (_isDisposed)
                throw new RunningTimeWatcherException(IsDisposedMessage);

            return _stopwatch.Elapsed;
        }

        /// <summary>
        ///     "Замораживает" время
        /// </summary>
        public void FreezeTime()
        {
            _stopwatch?.Stop();
        }

        /// <summary>
        ///     Продолжить
        /// </summary>
        public void Continue()
        {
            if (_isDisposed)
                throw new RunningTimeWatcherException(IsDisposedMessage);

            _stopwatch.Start();
        }

        protected override void Dispose(bool disposing)
        {
            _isDisposed = true;

            FreezeTime();
            _stopwatch?.Reset();
            _stopwatch = null;
        }
    }
}