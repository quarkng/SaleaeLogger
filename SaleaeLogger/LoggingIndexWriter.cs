using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SaleaeLogger
{
    public class LoggingIndexWriter
    {
        private LoggingIndexWriterThread thrd;

        public LoggingIndexWriter(Action<EventHandler<LoggingEventArgs>> registerHandler)
        {
            thrd = new LoggingIndexWriterThread();
            thrd.Start();
            registerHandler(thrd.Handler);
        }

        ~LoggingIndexWriter()
        {
            thrd.Kill();
        }
    }

    //=================================================================================
    public class LoggingIndexWriterThread
    {
        private string saveFolder;
        private string indexFile;

        private CancellationTokenSource canTokSrc;
        private Task task;
        private Dispatcher dispatcher;

        public EventHandler<LoggingEventArgs> Handler { get { return LoggingEventHandler; } }


        public void Start()
        {
            if( task == null )
            {
                canTokSrc = new CancellationTokenSource();
                task = Task.Factory.StartNew(() => { DoThread(canTokSrc.Token); }, canTokSrc.Token);
            }
        }

        public void Kill()
        {
            if (canTokSrc != null)
            {
                canTokSrc.Cancel();
            }

            if( dispatcher != null )
            {
                dispatcher.InvokeShutdown();
            }

            task = null;
        }

        private void DoThread(CancellationToken canTok)
        {
            Thread.CurrentThread.Name = "LoggingIndexWriterThread";

            dispatcher = Dispatcher.CurrentDispatcher;
            Dispatcher.Run();
        }
        
        private void LoggingEventHandler(Object sender, LoggingEventArgs e)
        {
            if( dispatcher != null )
            {
                dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    (ThreadStart)delegate() 
                    {
                        LoggingEventHandlerDispatched(sender,e);
                    });
            }
            else
            {   // Thread has not even started yet.
                LoggingEventHandlerDispatched(sender, e);
            }
        }

        private void LoggingEventHandlerDispatched(Object sender, LoggingEventArgs e)
        {
            if (e is LoggingFileEventArgs)
            {
                var ea = (LoggingFileEventArgs)e;
                if( indexFile != null )
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(indexFile, true))
                    {
                        file.WriteLine(ea.ScanTime.TotalSeconds + "," + Path.GetFileName(ea.File));
                        //file.WriteLine(ea.ScanTime.TotalSeconds + "," + ea.File);
                    }
                }
            }
            else if (e is LoggingStartedEventArgs)
            {
                var ea = (LoggingStartedEventArgs)e;
                saveFolder = ea.SavedToFolder;
                indexFile = System.IO.Path.Combine(saveFolder, "index.csv");
            }
            else if (e is LoggingStoppedEventArgs)
            {
                Kill();
            }
        }
    }
}
