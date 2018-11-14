using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaleaeAutomationApi;
using System.Windows;
using System.IO;
using System.Threading;

namespace SaleaeLogger
{
    class MainWindowViewModel : PropertyChangedNotifier
    {
        private SocketAPI saleae;
        private string host = "127.0.0.1";
        private int port = 10429;

        private ConnectedDevices[] connDevs;
        private ConnectedDevices dev;

        public int ScanSeconds { get; set; }
        public int BurstSeconds { get; set; }
        public bool HasConnection { get { return (saleae != null);} }

        private LoggingThread logger;
        private EventHandler<LoggingEventArgs> CallerLoggingEventHandler;

        public MainWindowViewModel()
        {

        }
        
        public void Connect(EventHandler<SaleaeStringEventArgs> saleaeApiMonitor = null,
            EventHandler<LoggingEventArgs> loggingEventHandler = null)
        {
            saleae = new SocketAPI(host, port);

            if (saleaeApiMonitor != null)
            {
                saleae.SaleaeStringEvent += saleaeApiMonitor;
            }
            CallerLoggingEventHandler = loggingEventHandler;

            connDevs = saleae.GetConnectedDevices();
            dev = (from d in connDevs where d.type == "LOGIC_PRO_16_DEVICE" select d).FirstOrDefault();
            if( dev.index == 0 )
            {   // Can't find logic pro 16.  Just pick the first one.
                var dLst = (from d in connDevs where d.index > 0 select d);
                var minIdx = (from d in dLst select d.index).Min();
                dev = (from d in dLst where d.index == minIdx select d ).FirstOrDefault();
            }

            if (dev.index != 0)
            {
                saleae.SelectActiveDevice(dev.index);
            }
            else
            {   // Can't find any device.  Not sure what is wrong.
                saleae = null;
            }

            OnPropertyChanged("HasConnection");
        }
        



        //=============================================================================

        internal void StartScan()
        {
            if( saleae == null )
            {   // Don't do anything if not yet connected!
                return;
            }

            if(BurstSeconds < 1)
            {
                BurstSeconds = 1;
            }

            StopScan();

            logger = new LoggingThread(saleae, ScanSeconds, BurstSeconds, CallerLoggingEventHandler);
            logger.StartScan();
        }


        internal void StopScan()
        {
            if (logger != null)
            {
                logger.StopScan();
                logger = null;
            }
        }

        //=============================================================================

    }
}
