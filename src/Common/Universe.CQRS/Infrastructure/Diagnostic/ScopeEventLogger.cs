using System;
using System.Collections.Generic;
using System.Text;
using Universe.Diagnostic.Logger;
using Universe.Types.Event;

namespace Universe.CQRS.Infrastructure.Diagnostic
{
    /// <summary>
    ///     Выполняет запись в лог.
    ///     Работает по событийно-ориентированному подходу с
    ///     <see cref="UniverseScope{TUniverseDbContext}"/>
    /// <author>Alex Envision</author>
    /// </summary>
    public class ScopeEventLogger : IUniverseLogger
    {
        private readonly IUniverseScope _scope;

        public event LogInfoDel LogInfo;

        public event LogErrorDel LogError;

        public event LogWarningDel LogWarning;

        public event LogTraceDel LogTrace;

        public ScopeEventLogger(IUniverseScope scope)
        {
            _scope = scope;
        }

        public void Info(string message)
        {
            this.LogInfo?.Invoke(
                new LogInfoEventArgs
                {
                    AllowReport = true,
                    Message = $"[{_scope.SessionId}] {message}"
                });
        }

        public void Info(string message, params object[] data)
        {
            this.LogInfo?.Invoke(
                new LogInfoEventArgs
                {
                    AllowReport = true,
                    Message = $"[{_scope.SessionId}] {message}",
                    Data = data
                });
        }

        public void Info(string message, bool allowReport)
        {
            this.LogInfo?.Invoke(
                new LogInfoEventArgs
                {
                    AllowReport = allowReport,
                    Message = $"[{_scope.SessionId}] {message}"
                });
        }

        public void Error(Exception ex, string message)
        {
            this.LogError?.Invoke(
                new LogErrorEventArgs
                {
                    Ex = ex,
                    AllowReport = true,
                    Message = $"[{_scope.SessionId}] {message}"
                });
        }

        public void Error(Exception ex, string message, params object[] data)
        {
            this.LogError?.Invoke(
                new LogErrorEventArgs
                {
                    Ex = ex,
                    AllowReport = true,
                    Message = $"[{_scope.SessionId}] {message}",
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
                    Message = $"[{_scope.SessionId}] {message}"
                });
        }

        public void Warning(string message)
        {
            this.LogWarning?.Invoke(
                new LogWarningEventArgs
                {
                    AllowReport = true,
                    Message = $"[{_scope.SessionId}] {message}"
                });
        }

        public void Warning(string message, params object[] data)
        {
            this.LogWarning?.Invoke(
                new LogWarningEventArgs
                {
                    AllowReport = true,
                    Message = $"[{_scope.SessionId}] {message}",
                    Data = data
                });
        }

        public void Warning(Exception ex, string message)
        {
            this.LogWarning?.Invoke(
                new LogWarningEventArgs
                {
                    AllowReport = true,
                    Message = $"[{_scope.SessionId}] {message}",
                    Ex = ex
                });
        }

        public void Warning(Exception ex, string message, params object[] data)
        {
            this.LogWarning?.Invoke(
                new LogWarningEventArgs
                {
                    AllowReport = true,
                    Message = $"[{_scope.SessionId}] {message}",
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
                    Message = $"[{_scope.SessionId}] {message}",
                    Ex = ex
                });
        }

        public void Warning(string message, bool allowReport)
        {
            this.LogWarning?.Invoke(
                new LogWarningEventArgs
                {
                    AllowReport = allowReport,
                    Message = $"[{_scope.SessionId}] {message}"
                });
        }

        public void Trace(string message)
        {
            this.LogTrace?.Invoke(
                new LogTraceEventArgs
                {
                    AllowReport = true,
                    Message = $"[{_scope.SessionId}] {message}"
                });
        }

        public void Trace(string message, params object[] data)
        {
            this.LogTrace?.Invoke(
                new LogTraceEventArgs
                {
                    AllowReport = true,
                    Message = $"[{_scope.SessionId}] {message}",
                    Data = data
                });
        }

        public void Trace(string message, bool allowReport)
        {
            this.LogTrace?.Invoke(
                new LogTraceEventArgs
                {
                    AllowReport = allowReport,
                    Message = $"[{_scope.SessionId}] {message}"
                });
        }
    }
}
