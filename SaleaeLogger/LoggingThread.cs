using SaleaeAutomationApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SaleaeLogger
{
    public class LoggingThread
    {
        private EventHandler<LoggingEventArgs> loggingEventHandler;

        private SocketAPI saleae;
        private LoggingIndexWriter indxWriter;

        public int ScanSeconds { get; private set; }
        public int BurstSeconds { get; private set; }

        private CancellationTokenSource canTokSrc;

        public LoggingThread(SocketAPI saleae, int ScanSeconds, int BurstSeconds, EventHandler<LoggingEventArgs> CallerLoggingEventHandler)
        {
            this.saleae = saleae;
            this.ScanSeconds = ScanSeconds;
            this.BurstSeconds = BurstSeconds;
            Register(CallerLoggingEventHandler);
        }

        public void StartScan()
        {
            StopScan();
            canTokSrc = new CancellationTokenSource();
            Task.Factory.StartNew(() => { LoggingScanThread(canTokSrc.Token, ScanSeconds); }, canTokSrc.Token);
        }

        public void StopScan()
        {
            if (canTokSrc != null)
            {
                canTokSrc.Cancel();
                canTokSrc = null;
            }
        }


        private void LoggingScanThread(CancellationToken canTok, int seconds)
        {
            Thread.CurrentThread.Name = "LoggingScanThread";

            int[] chans = { 0, 1, 2, 3, 4, 5 };
            int numchans = chans.Length;
            int numsamples = 4000000;

            saleae.SetActiveChannels(chans, null); //digital chans, analog chans

            // **** Turn off all triggers ****
            SaleaeAutomationApi.Trigger[] t = new SaleaeAutomationApi.Trigger[numchans];
            for (int i = 0; i < numchans; i++)
            {
                t[i] = SaleaeAutomationApi.Trigger.None;
            }
            t[5] = SaleaeAutomationApi.Trigger.Posedge;

            // Set up trigger
            saleae.SetTrigger(t);

            // **** Use lowest analog sample rate possible ****
            var sampRates = saleae.GetAvailableSampleRates();
            if (sampRates.Any(x => x.DigitalSampleRate == 4000000))
                saleae.SetSampleRate(sampRates.First(x => x.DigitalSampleRate == 4000000));

            saleae.SetNumSamples(BurstSeconds * 4000000);

            // **** Do the data logging ****
            DateTime started = DateTime.Now;

            string saveFolder = @"C:\_LogicData\" +
                started.Year + "_" + started.Month.ToString("00") + "_" + started.Day.ToString("00") + "-" +
                started.Hour.ToString("00") + "_" + started.Minute.ToString("00") + "_" + started.Second.ToString("00") +
                @"\";
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }

            indxWriter = new LoggingIndexWriter(Register);
            
            while (!canTok.IsCancellationRequested)
            {

                OnLoggingEvent(new LoggingStartedEventArgs(saveFolder));
                TimeSpan scanTime = DateTime.Now.Subtract(started);
                string filename = System.IO.Path.Combine(saveFolder, "data" + scanTime.TotalSeconds.ToString("00000") + ".logicdata");
                saleae.CaptureToFile(filename);
                OnLoggingEvent(new LoggingFileEventArgs(filename, scanTime));

                while(!saleae.IsProcessingComplete())
                {
                    if (canTok.IsCancellationRequested)
                    {   // Cancel me!
                        break;
                    }
                }

                OnLoggingEvent(new LoggingStoppedEventArgs());
            }

        }

        private void Register(EventHandler<LoggingEventArgs> handler)
        {
            loggingEventHandler -= handler; // Prevents double registration
            loggingEventHandler += handler;
        }

        private void OnLoggingEvent(LoggingEventArgs args)
        {
            if( loggingEventHandler != null )
            {
                loggingEventHandler(this, args);
            }
        }
    }

    //==================================================================
    public class LoggingEventArgs : EventArgs
    {
        public DateTime Timestamp { get; private set; }

        protected LoggingEventArgs()
        {
            Timestamp = DateTime.Now;
        }
    }

    public class LoggingStartedEventArgs : LoggingEventArgs
    {
        public string SavedToFolder { get; private set; }

        public LoggingStartedEventArgs(string saveFolder)
            : base()
        {
            SavedToFolder = saveFolder;
        }
    }

    public class LoggingStoppedEventArgs : LoggingEventArgs
    {
        public LoggingStoppedEventArgs()
            : base()
        {
        }
    }

    public class LoggingFileEventArgs : LoggingEventArgs
    {
        public string File { get; private set; }
        public TimeSpan ScanTime { get; private set; }

        public LoggingFileEventArgs(string file, TimeSpan scanTime)
            : base()
        {
            this.File = file;
            this.ScanTime = scanTime;
        }
    }

}
