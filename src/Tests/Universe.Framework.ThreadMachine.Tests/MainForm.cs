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
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Universe.Algorithm.MultiThreading;
using Universe.Diagnostic;
using Universe.Diagnostic.Logger;
using Universe.Framework.ThreadMachine.Tests.Multificator;
using Universe.Framework.ThreadMachine.Tests.Settings;
using Universe.Helpers.Extensions;
using Universe.IO.Extensions;
using Universe.Windows.Forms.Controls;
using Universe.Windows.Forms.Controls.Dialogs;
using Universe.Windows.Forms.Controls.Settings;

namespace Universe.Framework.ThreadMachine.Tests
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public partial class MainForm : Form
    {
        private EventLogger _log;

        protected Dictionary<long, PictureBox> PictureBoxes;

        protected int FreezeTimePeriod = 1000;

        private GeneralSettings _programSettings;

        protected Algorithm.MultiThreading.ThreadMachine SingleThreadMachine;

        protected Algorithm.MultiThreading.ThreadMachine MultiThreadMachine;

        private bool _isLogVisible;

        protected readonly Size MainFormSize;


        public MainForm()
        {
            InitializeComponent();

            PictureBoxes = new Dictionary<long, PictureBox> {
                { 1, pictureBox1 },
                { 2, pictureBox2 },
                { 3, pictureBox3 },
                { 4, pictureBox4 },
                { 5, pictureBox5 },
                { 6, pictureBox6 },
                { 7, pictureBox7 },
                { 8, pictureBox8 },
                { 9, pictureBox9 },
                { 10, pictureBox10 },
                { 11, pictureBox11 },
                { 12, pictureBox12 },
                { 13, pictureBox13 },
                { 14, pictureBox14 },
                { 15, pictureBox15 },
                { 16, pictureBox16 },

                { 17, pictureBox17 },
                { 18, pictureBox18 },
                { 19, pictureBox19 },
                { 20, pictureBox20 },
                { 21, pictureBox21 },
                { 22, pictureBox22 },
                { 23, pictureBox23 },
                { 24, pictureBox24 },
                { 25, pictureBox25 },
                { 26, pictureBox26 },
                { 27, pictureBox27 },
                { 28, pictureBox28 },
                { 29, pictureBox29 },
                { 30, pictureBox30 },
                { 31, pictureBox31 },
                { 32, pictureBox32 },

                { 33, pictureBox33 },
                { 34, pictureBox34 },
                { 35, pictureBox35 },
                { 36, pictureBox36 },
                { 37, pictureBox37 },
                { 38, pictureBox38 },
                { 39, pictureBox39 },
                { 40, pictureBox40 },
                { 41, pictureBox41 },
                { 42, pictureBox42 },
                { 43, pictureBox43 },
                { 44, pictureBox44 },
                { 45, pictureBox45 },
                { 46, pictureBox46 },
                { 47, pictureBox47 },
                { 48, pictureBox48 },

                { 49, pictureBox49 },
                { 50, pictureBox50 },
                { 51, pictureBox51 },
                { 52, pictureBox52 },
                { 53, pictureBox53 },
                { 54, pictureBox54 },
                { 55, pictureBox55 },
                { 56, pictureBox56 },
                { 57, pictureBox57 },
                { 58, pictureBox58 },
                { 59, pictureBox59 },
                { 60, pictureBox60 },
                { 61, pictureBox61 },
                { 62, pictureBox62 },
                { 63, pictureBox63 },
                { 64, pictureBox64 },

                { 65, pictureBox65 },
                { 66, pictureBox66 },
                { 67, pictureBox67 },
                { 68, pictureBox68 },
                { 69, pictureBox69 },
                { 70, pictureBox70 },
                { 71, pictureBox71 },
                { 72, pictureBox72 },
                { 73, pictureBox73 },
                { 74, pictureBox74 },
                { 75, pictureBox75 },
                { 76, pictureBox76 },
                { 77, pictureBox77 },
                { 78, pictureBox78 },
                { 79, pictureBox79 },
                { 80, pictureBox80 },

                { 81, pictureBox81 },
                { 82, pictureBox82 },
                { 83, pictureBox83 },
                { 84, pictureBox84 },
                { 85, pictureBox85 },
                { 86, pictureBox86 },
                { 87, pictureBox87 },
                { 88, pictureBox88 },
                { 89, pictureBox89 },
                { 90, pictureBox90 },
                { 91, pictureBox91 },
                { 92, pictureBox92 },
                { 93, pictureBox93 },
                { 94, pictureBox94 },
                { 95, pictureBox95 },
                { 96, pictureBox96 }
            };

            _log = new EventLogger();

            _log.LogInfo += e => {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message = $"[{currentDate}] {e.Message}{Environment.NewLine}";
                    this.SafeCall(() => this.tbLog.AppendText(message));
                }
            };

            _log.LogError += e => {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message = $"[{currentDate}] Во время выполнения операции произошла ошибка. Текст ошибки: {e.Message}.{Environment.NewLine} Трассировка стека: {e.Ex.StackTrace}{Environment.NewLine}";
                    this.SafeCall(() => this.tbLog.AppendText(message));
                }
            };

            MainFormSize = new Size(this.Size.Width, this.Size.Height);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _programSettings = _programSettings.Load();
            LoadOnForm();
        }

        public void LoadOnForm()
        {
            tbTempPath.Text = _programSettings.TemporaryFilesPath;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnLoadFromForm();
            _programSettings.Save();
        }

        public void UnLoadFromForm()
        {
            _programSettings.TemporaryFilesPath = tbTempPath.Text;
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            if (tbTempPath.Text.IsNullOrEmpty())
            {
                MessageBox.Show(@"Не указан путь к директории с временными файлами!");
                return;
            }

            btStart.Enabled = false;
            foreach (var kvp in PictureBoxes)
            {
                kvp.Value.BackColor = Color.LightGray;
            }

            var currDir = tbTempPath.Text;
            var tempFileDir = $"{currDir}\\TempFiles\\";
            if (!Directory.Exists(tempFileDir))
                Directory.CreateDirectory(tempFileDir);

            SingleThreadMachine = Algorithm.MultiThreading.ThreadMachine.Create(1).RunInMultiThreadsWithoutWaiting(
                () =>
                {
                    MultiThreadMachine = Algorithm.MultiThreading.ThreadMachine.Create(8);

                    var watchers = new ConcurrentDictionary<string, MTSpeedProcessingDataWatcher>();

                    foreach (var kvp in PictureBoxes)
                    {
                        var randomMachine = new Random();
                        var randomNumber = randomMachine.Next(2, 8);
                        ThreadStart code = () => {
                            var pb = kvp.Value;

                            var sessionId = $"TCM-{kvp.Key}";

                            try
                            {
                                using (RunningTimeWatcher watcher = new RunningTimeWatcher())
                                using (MTSpeedProcessingDataWatcher mtSpeedProcessingDataWatcher =
                                    new MTSpeedProcessingDataWatcher(watcher).AllowDisposeExternalWatcher())
                                {
                                    //speedProcessingDataWatcher.Disable();

                                    var freezyTime = randomNumber * FreezeTimePeriod;

                                    var generator = new Generator();
                                    long dim = freezyTime;
                                    var res = generator.Execute(dim);

                                    using (SpeedProcessingDataWatcher speedProcessingDataWatcher =
                                        new SpeedProcessingDataWatcher(watcher))
                                    {
                                        var generationSpeed = speedProcessingDataWatcher.EstimateCurrentSpeed(() => JsonConvert.SerializeObject(res));
                                        _log.Info($"{sessionId} Время генерации таблицы произведений: {generationSpeed.RunningTime}.");
                                        _log.Info($"{sessionId} Скорость генерации: {generationSpeed.Speed} Мб/сек.");
                                    }

                                    var normalizator = new Normalizator();
                                    var nres = normalizator.Execute(res);

                                        mtSpeedProcessingDataWatcher.EstimateCurrentSpeed(nres,
                                        normalizeSpeed => {
                                            _log.Info($"{sessionId} Время нормализации таблицы произведений: {normalizeSpeed.RunningTime}.");
                                            _log.Info($"{sessionId} Скорость нормализации: {normalizeSpeed.Speed} Мб/сек.");
                                        });

                                    //// Создаем 10% вероятность ошибки при вычислении данных
                                    //SetErrorProbability(10.0);

                                    var serializer = new MathSerializer();
                                    var serRes = serializer.SerializeMatrix(nres);

                                    mtSpeedProcessingDataWatcher.EstimateCurrentSpeed(() => XmlExtensions.SerializeObject(serRes), 
                                        serializeSpeed => {
                                        _log.Info($"{sessionId} Время сериализации таблицы произведений: {serializeSpeed.RunningTime}.");
                                        _log.Info($"{sessionId} Скорость сериализации: {serializeSpeed.Speed} Мб/сек.");
                                    });

                                    var fileName = $"{tempFileDir}\\MultificationTable-{kvp.Key}.txt";
                                    byte[] bufferBytes;
                                    using (var ms = new MemoryStream())
                                    {
                                        bufferBytes = Encoding.UTF8.GetBytes(serRes);
                                        ms.Write(bufferBytes, 0, bufferBytes.Length);

                                        // Создаем 10% вероятность ошибки при формировании файла
                                        //SetErrorProbability(10.0);

                                        using (var fs = File.Create(fileName))
                                        {
                                            ms.Position = 0;
                                            ms.CopyTo(fs);
                                            //fs.Write(bufferBytes, 0, bufferBytes.Length);
                                            fs.Flush();
                                            fs.Close();
                                        }

                                        mtSpeedProcessingDataWatcher.EstimateCurrentSpeed(bufferBytes,
                                            diskSpeed => {
                                                _log.Info($"{sessionId} Время сохранения на диск таблицы произведений: {diskSpeed.RunningTime}.");
                                                _log.Info($"{sessionId} Скорость сохранения на диск: {diskSpeed.Speed} Мб/сек.");
                                            });
                                    }

                                    var allProcessingData = new
                                    {
                                        res,
                                        nres,
                                        serRes,
                                        bufferBytes
                                    };

                                    var genericRunningTime = watcher.TakeRunningTime();

                                    mtSpeedProcessingDataWatcher.EstimateTotalSpeed(allProcessingData,
                                        (allProcessSpeed) =>
                                        {
                                            _log.Info($"{sessionId} Общая количество обработанных данных = {allProcessSpeed.DataSizeInMb} Мб.");
                                            _log.Info($"{sessionId} Общее время выполнения: {genericRunningTime}.");
                                            _log.Info($"{sessionId} Общая SPD = {allProcessSpeed.Speed} Мб/сек.");
                                        });
                                }

                                SetProcessedState(pb);
                            }
                            catch (Exception ex)
                            {
                                SetErrorState(pb);
                                Console.WriteLine(ex);
                                _log.Error(ex, ex.Message);
                            }
                        };
                        MultiThreadMachine.RunInMultiTheadsQueueWithoutWaiting(code);
                    }

                    btStart.SafeCall(() => btStart.Enabled = true);
                });
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Algorithm.MultiThreading.ThreadMachine.Create(1).RunInMultiThreadsWithoutWaiting(
                () => {
                    if (MultiThreadMachine != null && MultiThreadMachine.HasRunningQueueThreads())
                        MultiThreadMachine?.CancelAllThreads(true);

                    if (SingleThreadMachine != null && SingleThreadMachine.HasRunningThreads())
                        SingleThreadMachine?.CancelAllThreads(true);

                    btStart.SafeCall(() => btStart.Enabled = true);
                });
        }

        private void SetErrorProbability(double errorProb)
        {
            var randomMachine = new Random();
            var successfullyThreshold = Math.Abs(1.0 - randomMachine.NextDouble() * 100.0);
            if (successfullyThreshold < errorProb)
                throw new Exception("Во временя выполнения операции произошла ошибка!");
        }

        private void SetProcessedState(PictureBox pb)
        {
            pb.SafeCall(() => {
                pb.BackColor = Color.Green;
            });
        }

        private void SetErrorState(PictureBox pb)
        {
            pb.SafeCall(() => {
                pb.BackColor = Color.Red;
            });
        }

        private void btSetTempPath_Click(object sender, EventArgs e)
        {
            var tempPath = tbTempPath.Text;
            using (var fbd = new UniverseFolderBrowserDialog(tempPath))
            {
                if (fbd.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                tbTempPath.Text = fbd.SelectedPath;
            }
        }

        private void btHideLog_Click(object sender, EventArgs e)
        {
            if (_isLogVisible)
            {
                btLogExpander.Text = @"<<";
                tbLog.Visible = true;
                _isLogVisible = false;

                Size = new Size(this.Size.Width + tbLog.Width, this.Size.Height);
            }
            else
            {
                btLogExpander.Text = @">>";
                tbLog.Visible = false;
                _isLogVisible = true;

                Size = new Size(this.Size.Width - tbLog.Width, this.Size.Height);
            }
        }
    }
}