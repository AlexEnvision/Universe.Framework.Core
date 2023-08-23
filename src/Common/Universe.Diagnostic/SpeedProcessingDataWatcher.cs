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
using System.Linq;
using Universe.Diagnostic.Utilities;
using Universe.Types;

namespace Universe.Diagnostic
{
    /// <summary>
    ///     Счетчик скорости обработки данных (SPD)
    /// <author>Alex Envision</author>
    /// </summary>
    public class SpeedProcessingDataWatcher : DisposableObject
    {
        private readonly RunningTimeWatcher _internalRunningTimeWatcher;

        private readonly RunningTimeWatcher _externalRunningTimeWatcher;

        private List<EstimatedSpeedValue> _estimatedSpeedValues;

        private bool _allowDisposeExternal;

        private bool _disabled;

        /// <summary>
        ///     Конструктор класса <see cref="SpeedProcessingDataWatcher"/>
        /// </summary>
        /// <param name="runningTimeWatcher">Счетчик-наблюдатель времени выполнения</param>
        public SpeedProcessingDataWatcher(RunningTimeWatcher runningTimeWatcher)
        {
            _internalRunningTimeWatcher = new RunningTimeWatcher();
            _externalRunningTimeWatcher = runningTimeWatcher;

            _estimatedSpeedValues = new List<EstimatedSpeedValue>();
        }

        /// <summary>
        ///     Оценка скорости обработки
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Обработанные данные</param>
        private void EstimateSpeed<T>(T proceedData)
        {
            _internalRunningTimeWatcher.FreezeTime();
            var runningTime = _internalRunningTimeWatcher.TakeRunningTime();

            EstimateSpeedInternal(proceedData, runningTime);

            _internalRunningTimeWatcher.Continue();
        }

        private void EstimateSpeedInternal<T>(T proceedData, TimeSpan runningTime)
        {
            if (_disabled)
                return;

            // Получаем размер в байтах
            long size = proceedData.SizeInBytes();
            var sizeMb = (size * 1.0) / (1048576 * 1.0);

            var diffTotalSeconds = runningTime.TotalSeconds;

            var speed = sizeMb / diffTotalSeconds;

            var estimatedValue = new EstimatedSpeedValue
            {
                RunningTime = runningTime,
                Speed = speed,
                DataSizeBytes = size
            };

            _estimatedSpeedValues.Add(estimatedValue);
        }

        /// <summary>
        ///     Получить среднию скорость обработки данных в Мб/сек
        /// </summary>
        /// <returns></returns>
        private EstimatedSpeedValue TakeAverageSpeed()
        {
            var averageSpeed = _estimatedSpeedValues.Select(x => x.Speed).Sum() / _estimatedSpeedValues.Count * 1.0;
            var averageTicks = _estimatedSpeedValues.Select(x => x.RunningTime.Ticks).Sum() * 1.0 / _estimatedSpeedValues.Count * 1.0;
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
        private EstimatedSpeedValue TakeCurrentSpeed()
        {
            return _estimatedSpeedValues.LastOrDefault();
        }

        /// <summary>
        ///     Оценить и получить среднию скорость обработки данных в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Обработанные данные</param>
        /// <returns></returns>
        public EstimatedSpeedValue EstimateAverageSpeed<T>(T proceedData)
        {
            EstimateSpeed(proceedData);
            return TakeAverageSpeed();
        }

        /// <summary>
        ///     Оценить и получить среднию скорость обработки данных в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getProceedDataFunc">Функтор, возвращающий обработанные данные</param>
        /// <returns></returns>
        public EstimatedSpeedValue EstimateAverageSpeed<T>(Func<T> getProceedDataFunc)
        {
            var proceedData = getProceedDataFunc.Invoke();
            EstimateSpeed(proceedData);
            return TakeAverageSpeed();
        }

        /// <summary>
        ///     Оценить и получить последнюю зарегистрированную скорость обработки данных скорость в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Обработанные данные</param>
        /// <returns></returns>
        public EstimatedSpeedValue EstimateCurrentSpeed<T>(T proceedData)
        {
            EstimateSpeed(proceedData);
            return TakeCurrentSpeed();
        }

        /// <summary>
        ///     Оценить и получить последнюю зарегистрированную скорость обработки данных скорость в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getProceedDataFunc">Функтор, возвращающий обработанные данные</param>
        /// <returns></returns>
        public EstimatedSpeedValue EstimateCurrentSpeed<T>(Func<T> getProceedDataFunc)
        {
            var proceedData = getProceedDataFunc.Invoke();
            EstimateSpeed(proceedData);
            return TakeCurrentSpeed();
        }

        /// <summary>
        ///      Оценить и получить скорость обработки данных за всё время выполнения в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="proceedData">Обработанные данные</param>
        /// <returns></returns>
        public EstimatedSpeedValue EstimateTotalSpeed<T>(T proceedData)
        {
            _externalRunningTimeWatcher.FreezeTime();
            var runningTime = _internalRunningTimeWatcher.TakeRunningTime();

            EstimateSpeedInternal(proceedData, runningTime);

            _externalRunningTimeWatcher.Continue();
            return TakeCurrentSpeed();
        }

        /// <summary>
        ///      Оценить и получить скорость обработки данных за всё время выполнения в Мб/сек
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getProceedDataFunc">Функтор, возвращающий обработанные данные</param>
        /// <returns></returns>
        public EstimatedSpeedValue EstimateTotalSpeed<T>(Func<T> getProceedDataFunc)
        {
            _externalRunningTimeWatcher.FreezeTime();
            var runningTime = _internalRunningTimeWatcher.TakeRunningTime();

            var proceedData = getProceedDataFunc.Invoke();
            EstimateSpeedInternal(proceedData, runningTime);

            _externalRunningTimeWatcher.Continue();
            return TakeCurrentSpeed();
        }

        /// <summary>
        ///     Разрешает утилизацию внешнего <see cref="_internalRunningTimeWatcher"/>
        /// </summary>
        /// <returns></returns>
        public SpeedProcessingDataWatcher AllowDisposeExternalWatcher()
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
        ///     Сброс
        /// </summary>
        public void Reset()
        {
            _estimatedSpeedValues = new List<EstimatedSpeedValue>();
        }

        protected override void Dispose(bool disposing)
        {
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