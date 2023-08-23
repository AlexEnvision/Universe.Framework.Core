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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Universe.Diagnostic.Exceptions
{
    /// <summary>
    ///     Исключение, возникающее при ошибке связанной со счетчиком (наблюдателем) времени  
    /// </summary>
    public class RunningTimeWatcherException : Exception
    {
        /// <summary>
        /// Running Time Watcher message
        /// </summary>
        public string RunningTimeWatcherMessage { get; }

        /// <summary>
        /// Running Time Watcher resource name
        /// </summary>
        public string RunningTimeWatcherResourceName { get; }
        /// <summary>
        ///  Running Time Watcher validation errors
        /// </summary>
        public IList<string> RunningTimeWatcherValidationErrors { get; }

        /// <summary>
        ///  Initializes a new instance of the <see cref="RunningTimeWatcherException" /> class.
        /// </summary>
        public RunningTimeWatcherException() { }

        /// <summary>
        /// Running Time Watcher Exception
        /// </summary>
        /// <param name="message">message</param>
        public RunningTimeWatcherException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Running Time Watcher Exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public RunningTimeWatcherException(string message, Exception ex)
            : base(message, ex)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RunningTimeWatcherException" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="resourceName"></param>
        /// <param name="validationErrors"></param>
        public RunningTimeWatcherException(string message, string resourceName, IList<string> validationErrors)
            : base(message)
        {
            RunningTimeWatcherMessage = message;

            RunningTimeWatcherResourceName = resourceName;
            RunningTimeWatcherValidationErrors = validationErrors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RunningTimeWatcherException" /> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected RunningTimeWatcherException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            RunningTimeWatcherResourceName = info.GetString("RunningTimeWatcherResourceName");
            RunningTimeWatcherValidationErrors = (IList<string>)info.GetValue("RunningTimeWatcherValidationErrors", typeof(IList<string>));
        }

        /// <summary>
        /// Get object data by streaming context
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("RunningTimeWatcherResourceName", RunningTimeWatcherResourceName);

            info.AddValue("RunningTimeWatcherValidationErrors", RunningTimeWatcherValidationErrors, typeof(IList<string>));

            base.GetObjectData(info, context);
        }
    }
}