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
using Universe.Types.Event;

namespace Universe.Diagnostic.Logger
{
    /// <summary>
    ///     Выполняет запись в лог.
    ///     Работает по событийно-ориентированному подходу
    /// <author>Alex Envision</author>
    /// </summary>
    public class EventLogger : IUniverseLogger
    {
        public event LogInfoDel LogInfo;

        public event LogErrorDel LogError;

        public event LogWarningDel LogWarning;

        public event LogTraceDel LogTrace;

        public void Info(string message)
        {
            this.LogInfo?.Invoke(
                new LogInfoEventArgs
                {
                    AllowReport = true,
                    Message = message
                });
        }

        public void Info(string message, params object[] data)
        {
            this.LogInfo?.Invoke(
                new LogInfoEventArgs
                {
                    AllowReport = true,
                    Message = message,
                    Data = data
                });
        }

        public void Info(string message, bool allowReport)
        {
            this.LogInfo?.Invoke(
                new LogInfoEventArgs
                {
                    AllowReport = allowReport,
                    Message = message
                });
        }

        public void Error(Exception ex, string message)
        {
            this.LogError?.Invoke(
                new LogErrorEventArgs
                {
                    Ex = ex,
                    AllowReport = true,
                    Message = message
                });
        }

        public void Error(Exception ex, string message, params object[] data)
        {
            this.LogError?.Invoke(
                new LogErrorEventArgs
                {
                    Ex = ex,
                    AllowReport = true,
                    Message = message,
                    Data = data
                });
        }

        public void Error(Exception ex, string message, bool allowReport)
        {
            this.LogError?.Invoke(
                new LogErrorEventArgs
                {
                    Ex = ex,
                    AllowReport = allowReport,
                    Message = message
                });
        }

        public void Warning(string message)
        {
            this.LogWarning?.Invoke(
                new LogWarningEventArgs
                {
                    AllowReport = true,
                    Message = message
                });
        }

        public void Warning(string message, params object[] data)
        {
            this.LogWarning?.Invoke(
                new LogWarningEventArgs
                {
                    AllowReport = true,
                    Message = message,
                    Data = data
                });
        }

        public void Warning(Exception ex, string message)
        {
            this.LogWarning?.Invoke(
                new LogWarningEventArgs
                {
                    AllowReport = true,
                    Message = message,
                    Ex = ex
                });
        }

        public void Warning(Exception ex, string message, params object[] data)
        {
            this.LogWarning?.Invoke(
                new LogWarningEventArgs
                {
                    AllowReport = true,
                    Message = message,
                    Ex = ex,
                    Data = data
                });
        }

        public void Warning(Exception ex, string message, bool allowReport)
        {
            this.LogWarning?.Invoke(
                new LogWarningEventArgs
                {
                    AllowReport = allowReport,
                    Message = message,
                    Ex = ex
                });
        }

        public void Warning(string message, bool allowReport)
        {
            this.LogWarning?.Invoke(
                new LogWarningEventArgs
                {
                    AllowReport = allowReport,
                    Message = message
                });
        }

        public void Trace(string message)
        {
            this.LogTrace?.Invoke(
                new LogTraceEventArgs
                {
                    AllowReport = true,
                    Message = message
                });
        }

        public void Trace(string message, params object[] data)
        {
            this.LogTrace?.Invoke(
                new LogTraceEventArgs
                {
                    AllowReport = true,
                    Message = message,
                    Data = data
                });
        }

        public void Trace(string message, bool allowReport)
        {
            this.LogTrace?.Invoke(
                new LogTraceEventArgs
                {
                    AllowReport = allowReport,
                    Message = message
                });
        }
    }
}