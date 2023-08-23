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
using Universe.Diagnostic;
using Universe.Diagnostic.Utilities;
using Universe.Types;

namespace Universe.Algorithm.MultiThreading
{
    /// <summary>
    ///     Счетчик скорости обработки данных (SPD).
    ///     Вариант с выполнением оценки в отдельных потоках
    /// <author>Alex Envision</author>
    /// </summary>
    public class MTSpeedProcessingDataWatcher : DisposableObject
    {
        private readonly RunningTimeWatcher _internalRunningTimeWatcher;

        private readonly RunningTimeWatcher _externalRunningTimeWatcher;

        private ThreadMachine _threadMachine;

        private readonly int _threadsCount;

        private ConcurrentDictionary<int, EstimatedSpeedValue> _estimatedSpeedValues;

        private bool _allowDisposeExternal;

        private bool _disabled;

        /// <summary>
        ///     Период ожидания при наличии очереди выполняющихся потоков в миллисекундах
        /// </summary>
        private const int AwaitPeriod = 1000;

        private int _threadIndex;

        /// <summary>
        ///      Конструктор класса <see cref="MTSpeedProcessingDataWatcher"/>
        /// </summary>
        /// <param name="runningTimeWatcher">Счетчик-наблюдатель времени выполнения</param>
        public MTSpeedProcessingDataWatcher(RunningTimeWatcher runningTimeWatcher)
        {
            _externalRunningTimeWatcher = runningTimeWatcher;
            _internalRunningTimeWatcher = new RunningTimeWatcher();

            _threadsCount = 32;
            _threadIndex = 0;
            _estimatedSpeedValues = new ConcurrentDictionary<int, EstimatedSpeedValue>();
        }

        /// <summary>
        ///      Конструктор класса <see cref="MTSpeedProcessingDataWatcher"/>
        /// </summary>
        /// <param name="runningTimeWatcher">Счетчик-наблюдатель времени выполнения</param>
        /// <param name="threadsCount">Число потоков</param>
        public MTSpeedProcessingDataWatcher(RunningTimeWatcher runningTimeWatcher, int threadsCount)
        {
            _internalRunningTimeWatcher = new RunningTimeWatcher();

            _externalRunningTimeWatcher = runningTimeWatcher;
            _threadsCount = threadsCount;
            _threadIndex = 0;
            _estimatedSpeedValues = new ConcurrentDictionary<int, EstimatedSpeedValue>();
        }

        /// <summary>
        ///     Разрешает утилизацию внешнего <see cref="RunningTimeWatcher"/>
        /// </summary>
        /// <returns></returns>
        public MTSpeedProcessingDataWatcher AllowDisposeExternalWatcher()
        {
            _allowDisposeExternal = true;
            return this;
        }

        /// <summary>
        ///     Выключает оценку и действия после оценки
        /// </summary>
        public void Disable()
        {
            _disabled = true;
        }

        /// <summary>
        ///      Оценка скорости обработки
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Обработанные данные</param>
        /// <param name="afterEstimateAction">Действие, выполняемое после оценки</param>
        private void EstimateSpeedAsyncInternal<T>(T proceedData, Action<int> afterEstimateAction)
        {
            if (_disabled)
                return;

            var runningTime = _internalRunningTimeWatcher.TakeRunningTime();
            _internalRunningTimeWatcher.Reset();
            _internalRunningTimeWatcher.Continue();

            _threadMachine = _threadMachine == null
                ? ThreadMachine.Create(_threadsCount)
                    .AddAndRunInMultiThreadsWithoutWaiting(() => {
                        var threadIndex = _threadIndex++;
                        var estimatedValue = EstimateSpeedCoreInternal(proceedData, runningTime);
                        _estimatedSpeedValues.GetOrAdd(threadIndex, estimatedValue);

                        afterEstimateAction.Invoke(threadIndex);
                    })
                : _threadMachine
                    .AddAndRunInMultiThreadsWithoutWaiting(() => {
                        var threadIndex = _threadIndex++;
                        var estimatedValue = EstimateSpeedCoreInternal(proceedData, runningTime);
                        _estimatedSpeedValues.GetOrAdd(threadIndex, estimatedValue);

                        afterEstimateAction.Invoke(threadIndex);
                    });
        }

        /// <summary>
        ///      Оценка скорости обработки
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getProceedDataFunc">Функтор, возвращающий обработанные данные</param>
        /// <param name="afterEstimateAction">Действие, выполняемое после оценки</param>
        private void EstimateSpeedAsyncInternal<T>(Func<T> getProceedDataFunc, Action<int> afterEstimateAction)
        {
            if (_disabled)
                return;

            var runningTime = _internalRunningTimeWatcher.TakeRunningTime();
            _internalRunningTimeWatcher.Reset();
            _internalRunningTimeWatcher.Continue();

            _threadMachine = _threadMachine == null
                ? ThreadMachine.Create(_threadsCount)
                    .AddAndRunInMultiThreadsWithoutWaiting(() => {
                        var threadIndex = _threadIndex++;

                        var proceedData = getProceedDataFunc.Invoke();
                        var estimatedValue = EstimateSpeedCoreInternal(proceedData, runningTime);
                        _estimatedSpeedValues.GetOrAdd(threadIndex, estimatedValue);

                        afterEstimateAction.Invoke(threadIndex);
                    })
                : _threadMachine
                    .AddAndRunInMultiThreadsWithoutWaiting(() => {
                        var threadIndex = _threadIndex++;

                        var proceedData = getProceedDataFunc.Invoke();
                        var estimatedValue = EstimateSpeedCoreInternal(proceedData, runningTime);
                        _estimatedSpeedValues.GetOrAdd(threadIndex, estimatedValue);

                        afterEstimateAction.Invoke(threadIndex);
                    });
        }

        /// <summary>
        ///      Общая оценка скорости обработки
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Обработанные данные</param>
        /// <param name="afterEstimateAction">Действие, выполняемое после оценки</param>
        private void EstimateSpeedTotalAsyncInternal<T>(T proceedData, Action<int> afterEstimateAction)
        {
            if (_disabled)
                return;

            var runningTime = _externalRunningTimeWatcher.TakeRunningTime();

            _threadMachine = _threadMachine == null
                ? ThreadMachine.Create(_threadsCount)
                    .AddAndRunInMultiThreadsWithoutWaiting(() => {
                        var threadIndex = _threadIndex++;
                        var estimatedValue = EstimateSpeedCoreInternal(proceedData, runningTime);
                        _estimatedSpeedValues.GetOrAdd(threadIndex, estimatedValue);

                        afterEstimateAction.Invoke(threadIndex);
                    })
                : _threadMachine
                    .AddAndRunInMultiThreadsWithoutWaiting(() => {
                        var threadIndex = _threadIndex++;
                        var estimatedValue = EstimateSpeedCoreInternal(proceedData, runningTime);
                        _estimatedSpeedValues.GetOrAdd(threadIndex, estimatedValue);

                        afterEstimateAction.Invoke(threadIndex);
                    });
        }

        /// <summary>
        ///      Общая оценка скорости обработки
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getProceedDataFunc">Функтор, возвращающий обработанные данные</param>
        /// <param name="afterEstimateAction">Действие, выполняемое после оценки</param>
        private void EstimateSpeedTotalAsyncInternal<T>(Func<T> getProceedDataFunc, Action<int> afterEstimateAction)
        {
            if (_disabled)
                return;

            var runningTime = _externalRunningTimeWatcher.TakeRunningTime();

            _threadMachine = _threadMachine == null
                ? ThreadMachine.Create(_threadsCount)
                    .AddAndRunInMultiThreadsWithoutWaiting(() => {
                        var threadIndex = _threadIndex++;

                        var proceedData = getProceedDataFunc.Invoke();
                        var estimatedValue = EstimateSpeedCoreInternal(proceedData, runningTime);
                        _estimatedSpeedValues.GetOrAdd(threadIndex, estimatedValue);

                        afterEstimateAction.Invoke(threadIndex);
                    })
                : _threadMachine
                    .AddAndRunInMultiThreadsWithoutWaiting(() => {
                        var threadIndex = _threadIndex++;

                        var proceedData = getProceedDataFunc.Invoke();
                        var estimatedValue = EstimateSpeedCoreInternal(proceedData, runningTime);
                        _estimatedSpeedValues.GetOrAdd(threadIndex, estimatedValue);

                        afterEstimateAction.Invoke(threadIndex);
                    });
        }

        private EstimatedSpeedValue EstimateSpeedCoreInternal<T>(T proceedData, TimeSpan runningTime)
        {
            // Получаем размер в байтах
            long size = proceedData.SizeInBytes();
            var sizeMb = (size * 1.0) / (1048576 * 1.0);

            var runningTimeTotalSeconds = runningTime.TotalSeconds;
            var speed = sizeMb / runningTimeTotalSeconds;

            var estimatedValue = new EstimatedSpeedValue {
                RunningTime = runningTime,
                Speed = speed,
                DataSizeBytes = size
            };
           
            return estimatedValue;
        }

        /// <summary>
        ///     Получить среднию скорость обработки данных в Мб/сек
        /// </summary>
        /// <returns></returns>
        private EstimatedSpeedValue TakeAverageSpeedAndOther()
        {
            var averageSpeed = _estimatedSpeedValues.Select(x => x.Value.Speed).Sum() / _estimatedSpeedValues.Count * 1.0;
            var averageTicks = _estimatedSpeedValues.Select(x => x.Value.RunningTime.Ticks).Sum() * 1.0 / _estimatedSpeedValues.Count * 1.0;
            var ticks = (long)averageTicks;

            return new EstimatedSpeedValue
            {
                Speed = averageSpeed,
                RunningTime = new TimeSpan(ticks)
            };
        }

        /// <summary>
        ///     Получить последнюю зарегистрированную скорость обработки данных скорость в Мб/сек
        /// </summary>
        /// <returns></returns>
        private EstimatedSpeedValue TakeCurrentSpeedAndOther(int threadIndex)
        {
            _estimatedSpeedValues.TryGetValue(threadIndex, out var estimatedSpeedValue);
            return estimatedSpeedValue;
        }

        /// <summary>
        ///     Ожидание
        /// </summary>
        /// <param name="awaitPeriod">Период ожидания</param>
        private void Await(int awaitPeriod)
        {
            Thread.Sleep(awaitPeriod);
        }

        /// <summary>
        ///     Оценить и получить последнюю зарегистрированную скорость обработки данных скорость в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Обработанные данные</param>
        /// <param name="afterEstimateAction">Действие, выполняемое после оценки</param>
        /// <returns></returns>
        public void EstimateCurrentSpeed<T>(T proceedData, Action<EstimatedSpeedValue> afterEstimateAction)
        {
            EstimateSpeedAsyncInternal(proceedData, (indexT) => {
                var result = TakeCurrentSpeedAndOther(indexT);
                if (result != null)
                    afterEstimateAction.Invoke(result);
            });
        }

        /// <summary>
        ///     Оценить и получить последнюю зарегистрированную скорость обработки данных скорость в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Функтор, возвращающий обработанные данные</param>
        /// <param name="afterEstimateAction">Действие, выполняемое после оценки</param>
        /// <returns></returns>
        public void EstimateCurrentSpeed<T>(Func<T> proceedData, Action<EstimatedSpeedValue> afterEstimateAction)
        {
            EstimateSpeedAsyncInternal(proceedData, (indexT) => {
                var result = TakeCurrentSpeedAndOther(indexT);
                if (result != null)
                    afterEstimateAction.Invoke(result);
            });
        }

        /// <summary>
        ///     Оценить и получить среднию скорость обработки данных в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Обработанные данные</param>
        /// <param name="afterEstimateAction">Действие, выполняемое после оценки</param>
        /// <returns></returns>
        public void EstimateAverageSpeed<T>(T proceedData, Action<EstimatedSpeedValue> afterEstimateAction)
        {
            EstimateSpeedAsyncInternal(proceedData, (indexT) => {
                var result = TakeAverageSpeedAndOther();
                if (result != null)
                    afterEstimateAction.Invoke(result);
            });
        }

        /// <summary>
        ///     Оценить и получить среднию скорость обработки данных в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Функтор, возвращающий обработанные данные</param>
        /// <param name="afterEstimateAction">Действие, выполняемое после оценки</param>
        /// <returns></returns>
        public void EstimateAverageSpeed<T>(Func<T> proceedData, Action<EstimatedSpeedValue> afterEstimateAction)
        {
            EstimateSpeedAsyncInternal(proceedData, (indexT) => {
                var result = TakeAverageSpeedAndOther();
                if (result != null)
                    afterEstimateAction.Invoke(result);
            });
        }

        /// <summary>
        ///     Оценить и получить скорость обработки данных за всё время выполнения в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Обработанные данные</param>
        /// <param name="afterEstimateAction">Действие, выполняемое после оценки</param>
        /// <returns></returns>
        public void EstimateTotalSpeed<T>(T proceedData, Action<EstimatedSpeedValue> afterEstimateAction)
        {
            EstimateSpeedTotalAsyncInternal(proceedData, (indexT) => {
                var result = TakeCurrentSpeedAndOther(indexT);
                if (result != null)
                    afterEstimateAction.Invoke(result);
            });
        }

        /// <summary>
        ///     Оценить и получить скорость обработки данных за всё время выполнения в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Функтор, возвращающий обработанные данные</param>
        /// <param name="afterEstimateAction">Действие, выполняемое после оценки</param>
        /// <returns></returns>
        public void EstimateTotalSpeed<T>(Func<T> proceedData, Action<EstimatedSpeedValue> afterEstimateAction)
        {
            EstimateSpeedTotalAsyncInternal(proceedData, (indexT) => {
                var result = TakeCurrentSpeedAndOther(indexT);
                if (result != null)
                    afterEstimateAction.Invoke(result);
            });
        }

        /// <summary>
        ///     Сброс
        /// </summary>
        public void Reset()
        {
            if (_threadMachine == null)
                return;

            if (_threadMachine.HasRunningThreads())
            {
                Await(AwaitPeriod);
                Reset();
            }

            _estimatedSpeedValues = new ConcurrentDictionary<int, EstimatedSpeedValue>();
        }

        protected override void Dispose(bool disposing)
        {
            if (_threadMachine?.HasRunningThreads() ?? false)
            {
                Await(AwaitPeriod);
                Dispose(disposing);
            }

            _internalRunningTimeWatcher?.Dispose();

            if (_allowDisposeExternal)
                _externalRunningTimeWatcher?.Dispose();
        }

        public class EstimatedSpeedValue
        {
            public long DataSizeBytes { get; set; }
        
            public double DataSizeInMb => (DataSizeBytes * 1.0) / (1048576 * 1.0);

            public double Speed { get; set; }

            public TimeSpan RunningTime { get; set; }
        }
    }
}