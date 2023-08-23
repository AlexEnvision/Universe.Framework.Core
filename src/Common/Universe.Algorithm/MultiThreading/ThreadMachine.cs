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
using System.Linq;
using System.Threading;

namespace Universe.Algorithm.MultiThreading
{
    /// <summary>
    ///     Машина потоков
    /// <author>Alex Envision</author>
    /// </summary>
    public class ThreadMachine
    {
        private Thread[] _threadsPool;

        private ConcurrentDictionary<long, Thread> _threadsQueue;

        /// <summary>
        ///     Количество потоков
        /// </summary>
        private readonly int _threadsCount;

        /// <summary>
        ///     Получен сигнал на отмену всех операций
        /// </summary>
        private bool _isRecievedInterruptSignal;

        /// <summary>
        ///     Период обновления при наличии очереди потоков в миллисекундах
        /// </summary>
        private const int UpdatePeriod = 1000;

        private ThreadMachine(int threadsCount)
        {
            _threadsPool = new Thread[threadsCount];
            _threadsCount = threadsCount;
        }

        /// <summary>
        ///     Создание машины потоков
        /// </summary>
        /// <param name="threadsCount">Количество потоков</param>
        /// <returns></returns>
        public static ThreadMachine Create(int threadsCount)
        {
            return new ThreadMachine(threadsCount);
        }

        public ThreadMachine RunInMultiThreadsWithoutWaiting(ThreadStart start)
        {
            for (var index = 0; index < _threadsPool.Length; index++)
            {
                var thread = new Thread(start);
                _threadsPool[index] = thread;
                _threadsPool[index].Start();
            }

            return this;
        }

        /// <summary>
        ///     Добавление еще одного выполняющегося потока без ожидания его выполнения
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public ThreadMachine AddAndRunInMultiThreadsWithoutWaiting(ThreadStart start)
        {
            var allowReTry = false;
            for (var index = 0; index < _threadsPool.Length; index++)
            {
                var thread = new Thread(start);
                if (_threadsPool[index] == null)
                {
                    _threadsPool[index] = thread;
                    _threadsPool[index].Start();
                    break;
                }

                if (_threadsPool[index].ThreadState != ThreadState.Running)
                {
                    _threadsPool[index] = thread;
                    _threadsPool[index].Start();
                    break;
                }

                allowReTry = index == _threadsPool.Length;

            }

            // Если нет места в пуле потоков, то выжидаем период обновления
            // и пробуем снова добавить в пул потоков новый поток
            if (allowReTry)
            {
                Thread.Sleep(UpdatePeriod);
                return AddAndRunInMultiThreadsWithoutWaiting(start);
            }

            return this;
        }

        public void RunInMultiTheadsQueueWithoutWaiting(ThreadStart threadAction)
        {
            _threadsQueue = _threadsQueue ?? new ConcurrentDictionary<long, Thread>();
            if (_threadsQueue.Count >= _threadsCount)
            {
                if (_isRecievedInterruptSignal)
                    return;

                Thread.Sleep(UpdatePeriod);
                RemoveDeadThreads();
                RunInMultiTheadsQueueWithoutWaiting(threadAction);
            }
            else
            {
                if (_isRecievedInterruptSignal)
                    return;

                var maxExistedThreadIndex = _threadsQueue.Count > 0 ? _threadsQueue.Keys.Max() : 0;
                var threadIndex = maxExistedThreadIndex + 1;

                ThreadStart internalCoveredThreadAction = () => {
                    var internalThreadIndex = threadIndex;
                    try
                    {
                        threadAction.Invoke();
                    }
                    finally
                    {
                        if (!_threadsQueue.TryRemove(internalThreadIndex, out _))
                        {
                            RemoveDeadThreads();
                        }
                    }
                };

                var thread = new Thread(internalCoveredThreadAction);
                thread.Start();
                _threadsQueue.GetOrAdd(threadIndex, thread);
            }
        }

        /// <summary>
        ///     Удаляет "мертвые" потоки
        /// </summary>
        private void RemoveDeadThreads()
        {
            foreach (var thd in _threadsQueue)
            {
                if (!thd.Value.IsAlive)
                    _threadsQueue.TryRemove(thd.Key, out _);
            }
        }

        public void RunInMultiTheadsWhenAllCompleted(ThreadStart start)
        {
            for (var index = 0; index < _threadsPool.Length; index++)
            {
                var thread = new Thread(start);
                _threadsPool[index] = thread;
                _threadsPool[index].Start();
            }

            do { }
            while (!WhenAllCompleted(_threadsPool));
        }

        public void RunInMultiTheadsWithoutWaiting(params ThreadStart[] starts)
        {
            if (starts.Length == 0)
                return;

            _threadsPool = new Thread[starts.Length];
            for (var index = 0; index < _threadsPool.Length; index++)
            {
                var thread = new Thread(starts[index]);
                _threadsPool[index] = thread;
                _threadsPool[index].Start();
            }
        }

        public void RunInMultiTheadsWhenAllCompleted(params ThreadStart[] starts)
        {
            if (starts.Length == 0)
                return;

            _threadsPool = new Thread[starts.Length];
            for (var index = 0; index < _threadsPool.Length; index++)
            {
                var thread = new Thread(starts[index]);
                _threadsPool[index] = thread;
                _threadsPool[index].Start();
            }

            do { }
            while (!WhenAllCompleted(_threadsPool));
        }

        private bool WhenAllCompleted(Thread[] threadsPool)
        {
            var isCompleted = threadsPool.All(x => x.ThreadState != ThreadState.Running);
            return isCompleted;
        }

        public bool HasRunningThreads()
        {
            return _threadsPool?.Any(x => x?.ThreadState == ThreadState.Running) ?? false;
        }

        public bool HasRunningQueueThreads()
        {
            return _threadsQueue?.Any(x => x.Value.ThreadState == ThreadState.Running) ?? false;
        }

        public void Reset()
        {
            _isRecievedInterruptSignal = false;
        }

        public void CancelAllThreads()
        {
            _isRecievedInterruptSignal = true;

            if (_threadsPool != null)
                for (var index = 0; index < _threadsPool.Length; index++)
                {
                    _threadsPool[index]?.Abort();
                    Thread.Sleep(1000);
                    _threadsPool[index]?.Interrupt();
                }

            if (_threadsQueue != null)
                foreach (var thread in _threadsQueue)
                {
                    thread.Value?.Abort();
                    Thread.Sleep(1000);
                    thread.Value?.Interrupt();
                }
        }

        public void CancelAllThreads(bool supplyCancelThreadErrors)
        {
            this._isRecievedInterruptSignal = true;

            if (this._threadsPool != null)
                for (var index = 0; index < this._threadsPool.Length; index++)
                {
                    try
                    {
                        this._threadsPool[index]?.Abort();
                        Thread.Sleep(1000);
                        this._threadsPool[index]?.Interrupt();
                    }
                    catch (Exception)
                    {
                        if (!supplyCancelThreadErrors)
                            throw;
                    }
                }

            if (_threadsQueue != null)
                foreach (var thread in _threadsQueue)
                {
                    try
                    {
                        thread.Value?.Abort();
                        Thread.Sleep(1000);
                        thread.Value?.Interrupt();
                    }
                    catch (Exception)
                    {
                        if (!supplyCancelThreadErrors)
                            throw;
                    }
                }
        }
    }
}
