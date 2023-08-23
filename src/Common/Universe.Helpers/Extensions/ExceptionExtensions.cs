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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Universe.Helpers.Extensions
{
    /// <summary>
    ///     Расширения для <see cref="Exception"/>.
    /// <author>Alex Envision</author>
    /// </summary>
    public static class ExceptionExtensions
    {
        public static string AttemptErrorsExceptionDataKey = "AttemptErrors";

        private static readonly ConcurrentDictionary<Type, Func<Exception, string>> _exFormatFuncs =
            new ConcurrentDictionary<Type, Func<Exception, string>>();

        public static Exception AddAttemptErrors(
            this Exception exForThrow,
            List<Exception> attemptErrors,
            params Exception[] exForExclude)
        {
            if (exForThrow == null)
                throw new ArgumentNullException(nameof(exForThrow));
            if (attemptErrors == null)
                throw new ArgumentNullException(nameof(attemptErrors));

            var errors = attemptErrors.Where(_ => _ != exForThrow && !(exForExclude?.Contains(_) ?? false)).ToList();
            if (errors.Count > 0)
                exForThrow.Data.Add(AttemptErrorsExceptionDataKey, errors);

            return exForThrow;
        }

        /// <summary>
        /// Gets the exception information.
        /// </summary>
        /// <param name="exception">The ex.</param>
        /// <returns>Error: Message: {0} ExceptionType: {0} StackTrace: {0} + innerException</returns>
        public static string GetExceptionInfo(this Exception exception)
        {
            return exception.GetExceptionInfoMessageExceptionTypeStackTrace();
        }

        /// <summary>
        /// Gets the exception information.
        /// </summary>
        /// <param name="exception">The ex.</param>
        /// <returns>
        /// String in which all the exception messages are listed first. And then ExceptionType, StackTrace starting with
        /// the most inner exception.
        /// </returns>
        public static string GetExceptionInfoMessageExceptionTypeStackTrace(this Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var list = new List<ExceptionWithPath>();
            ExceptionToListWithPath(list, exception, string.Empty, 0);

            var sb1 = new StringBuilder();
            sb1.AppendLine($"Exception count:{list.Count} maximum nesting depth:{list.Min(_ => _.Level)}");
            sb1.AppendLine("Messages:");

            foreach (var exInf in list)
            {
                ExInfBuildMsgWithoutStackTrace(exInf, sb1);
            }

            sb1.AppendLine("StackTrace:");
            for (var index = list.Count - 1; index >= 0; index--)
            {
                var exInf = list[index];
                var ex = exInf.Exception;
                sb1.AppendLine($"---Exception {exInf.Path} [{ex.GetType().FullName}]:");
                sb1.AppendLine($"{ex.StackTrace}");
            }

            return sb1.ToString();
        }

        /// <summary>
        /// Gets the exception information only messages.
        /// </summary>
        /// <param name="exception">The ex.</param>
        /// <returns>String in which all the exception messages are listed first.</returns>
        public static string GetExceptionInfoOnlyMessages(this Exception exception)
        {
            var list = new List<ExceptionWithPath>();
            ExceptionToListWithPath(list, exception, string.Empty, 0);
            var sb1 = new StringBuilder();
            foreach (var exInf in list)
            {
                ExInfBuildMsgWithoutStackTrace(exInf, sb1);
            }

            return sb1.ToString();
        }

        /// <summary>
        ///     <see cref="OperationCanceledException"/>
        ///     <see cref="ThreadAbortException"/>
        ///     <see cref="StackOverflowException"/>
        ///     <see cref="OutOfMemoryException"/>
        ///     <see cref="AccessViolationException"/>
        ///     <see cref="ExecutionEngineException"/>
        ///     <see cref="BadImageFormatException"/>
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsRethrow(this Exception ex)
        {
            return ex is OperationCanceledException
                || ex is ThreadAbortException
                || ex is StackOverflowException
                || ex is OutOfMemoryException
                || ex is AccessViolationException
                || ex is ExecutionEngineException;
        }

        public static void RegFormatFunc<T>(Func<Exception, string> func)
        {
            _exFormatFuncs.TryAdd(typeof(T), func);
        }

        public static void Throw(this IList<Exception> exceptions, string message)
        {
            if (exceptions == null)
                throw new ArgumentNullException(nameof(exceptions));

            if (exceptions.Count > 0)
            {
                if (message == null)
                    message = string.Empty;
                message += "\r\n";

                throw new AggregateException($"{message}See AggregateException. Error count = {exceptions.Count}.", exceptions);
            }
        }

        public static void TryCatch(this IList<Exception> exceptions, Action action)
        {
            if (exceptions == null)
                throw new ArgumentNullException(nameof(exceptions));

            try
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                action();
            }
            catch (Exception ex)
            {
                if (ex.IsRethrow()) throw;

                exceptions.Add(ex);
            }
        }

        private static void ExceptionToListWithPath(List<ExceptionWithPath> list, Exception exception, string parentPath, int startLevel)
        {
            if (list.Any(_ => _.Exception.Equals(exception)))
                return;

            var curEx = exception;
            var curLevel = startLevel;
            while (curEx != null)
            {
                var curPath = $"{parentPath}lv{curLevel}";
                list.Add(
                    new ExceptionWithPath {
                        Level = curLevel,
                        Path = curPath,
                        Exception = curEx
                    });

                if (curEx is AggregateException aggregateException)
                    for (var index = 0; index < aggregateException.InnerExceptions.Count; index++)
                    {
                        ExceptionToListWithPath(list, aggregateException.InnerExceptions[index], $"{curPath}.InnerExceptions[{index}]", curLevel - 1);
                    }

                foreach (var key in curEx.Data.Keys)
                {
                    if (curEx.Data[key] is Exception innerDataEx)
                    {
                        ExceptionToListWithPath(list, innerDataEx, $"{curPath}.Data[{key}]", curLevel - 1);
                        continue;
                    }

                    var innerDataExs = (curEx.Data[key] as IEnumerable<Exception>)?.ToList();
                    if (innerDataExs != null)
                        for (var index = 0; index < innerDataExs.Count; index++)
                        {
                            ExceptionToListWithPath(list, innerDataExs[index], $"{curPath}.Data[{key}][{index}]", curLevel - 1);
                        }
                }

                curEx = curEx.InnerException;
                curLevel--;
            }
        }

        private static void ExInfBuildMsgWithoutStackTrace(ExceptionWithPath exInf, StringBuilder sb)
        {
            var ex = exInf.Exception;
            sb.Append($"---Exception {exInf.Path} [{ex.GetType().FullName}]");
            if (ex is AggregateException aggregateException)
                sb.Append($" InnerExceptions.Count:{aggregateException.InnerExceptions.Count}");
            if (ex.Data.Keys.Count > 0)
                sb.AppendLine($" Data.Count:{ex.Data.Keys.Count}");
            if (ex.Data.Contains(AttemptErrorsExceptionDataKey))
                sb.AppendLine($" Data contains {AttemptErrorsExceptionDataKey}");
            sb.AppendLine(" msg:");

            sb.AppendLine($"{ex.Message}");

            var exceptionInfo = GetFormatExceptionInfo(ex);
            if (!exceptionInfo.IsNullOrEmpty())
                sb.AppendLine($"ExceptionInfo: {exceptionInfo}");
        }

        private static string GetFormatExceptionInfo(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            var exceptionType = ex.GetType();
            Func<Exception, string> formatFunc = null;
            while (true)
            {
                Debug.Assert(exceptionType != null, nameof(exceptionType) + " != null");
                _exFormatFuncs.TryGetValue(exceptionType, out formatFunc);

                if (formatFunc != null) break;

                exceptionType = exceptionType.BaseType;

                if (exceptionType == typeof(object))
                    break;
            }

            return formatFunc?.Invoke(ex);
        }

        private class ExceptionWithPath
        {
            public Exception Exception { get; set; }

            public int Level { get; set; }

            public string Path { get; set; }
        }
    }
}