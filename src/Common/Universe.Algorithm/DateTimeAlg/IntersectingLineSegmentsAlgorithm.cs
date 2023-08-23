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

namespace Universe.Algorithm.DateTimeAlg
{
    /// <summary>
    ///     Алгоритмм пересекающихся отрезков
    /// <author>Alex Envision</author>
    /// </summary>
    public class IntersectingLineSegmentsAlgorithm
    {
        /// <summary>
        /// Get possible period in crossing multiplicity period in specified period
        /// </summary>
        /// <param name="crossPeriodes">Crossing periodes in specified period</param>
        /// <param name="start">The searching period date of beginning</param>
        /// <param name="end">The searching period date of ending</param>
        /// <returns></returns>
        protected TimePeriod PossiblePeriodSearch(
            List<TimePeriod> crossPeriodes,
            DateTime start,
            DateTime end)
        {
            if (crossPeriodes.Count == 0)
                return null;

            var cursorDate = start;
            var sortedCrossPeriod = crossPeriodes.OrderBy(x => x.StartDateTime).ToList();

            var points = new List<DateTime>();
            while (cursorDate <= end)
            {
                points.Add(cursorDate);
                cursorDate = cursorDate.AddDays(1);
            }

            var nonCrossedPoints = new List<DateTime>();
            nonCrossedPoints.AddRange(points);
            foreach (var point in points)
            {
                if (sortedCrossPeriod.Any(timePeriod => point >= timePeriod.StartDateTime && point <= timePeriod.EndDateTime))
                {
                    nonCrossedPoints.Remove(point);
                }
            }

            if (nonCrossedPoints.Count == 0)
                return null;

            if (nonCrossedPoints.Count == 1)
                return new TimePeriod { StartDateTime = nonCrossedPoints.FirstOrDefault(), EndDateTime = nonCrossedPoints.FirstOrDefault() };

            var prevPoint = nonCrossedPoints.FirstOrDefault();
            var firstAvailablePoints = new List<DateTime> { prevPoint };
            for (var index = 1; index < nonCrossedPoints.Count; index++)
            {
                var nonCrossedPoint = nonCrossedPoints[index];
                var expectedPoint = prevPoint.AddDays(1);
                if (nonCrossedPoint == expectedPoint)
                    firstAvailablePoints.Add(nonCrossedPoint);
                else
                    break;

                prevPoint = nonCrossedPoint;
            }

            return new TimePeriod { StartDateTime = firstAvailablePoints.FirstOrDefault(), EndDateTime = firstAvailablePoints.LastOrDefault() };
        }

        public class TimePeriod
        {
            public DateTime StartDateTime { get; set; }

            public DateTime EndDateTime { get; set; }
        }
    }
}