using SaleaeAutomationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SaleaeLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DateTime connectedTime = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.Connect(SaleaeEventHandler, LoggingEventHandler);
                connectedTime = DateTime.Now;
            }
            catch
            {
                MessageBox.Show("Connection failed.  Please check that Saleae Logic is running and socket server is enabled for port 10429.",
                    "Connect Failed");
            }
        }

        private void StartLogging_Click(object sender, RoutedEventArgs e)
        {
            vm.StartScan();
        }

        private void StopLogging_Click(object sender, RoutedEventArgs e)
        {
            vm.StopScan();
            tbMonitor.Inlines.Add(new Run("Stop Logging Requested\n") { Foreground = Brushes.Crimson, Background = Brushes.Yellow });
        }

        private void SaleaeEventHandler(Object sender, SaleaeStringEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    DispatchedSaleaeEventHandler(e);
                });
        }

        private void DispatchedSaleaeEventHandler(SaleaeStringEventArgs e)
        {
            if (e is SeleaeWriteEventArgs)
            {
                tbMonitor.Inlines.Add(new Run("==== " + e.Timestamp.Subtract(connectedTime).ToString() + " ====\n") { Foreground = Brushes.DarkRed });
                tbMonitor.Inlines.Add(new Run(e.Value + "\n") { Foreground = Brushes.CornflowerBlue });
            }
            else if (e is SeleaeReadEventArgs)
            {
                tbMonitor.Inlines.Add(new Run(e.Value + "\n") { Foreground = Brushes.Black });
            }

            // Purge very old lines
            while (tbMonitor.Inlines.Count > 300)
            {
                tbMonitor.Inlines.Remove(tbMonitor.Inlines.FirstInline);
            }
            scrollMonitor.ScrollToBottom();
        }

        private void LoggingEventHandler(Object sender, LoggingEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            (ThreadStart)delegate()
            {
                DispatchedLoggingEventHandler(e);
            });
        }

        private void DispatchedLoggingEventHandler(LoggingEventArgs e)
        {
            if (e is LoggingFileEventArgs)
            {   // Do nothing
            }
            else if (e is LoggingStartedEventArgs)
            {
                tbMonitor.Inlines.Add(new Run("Logging Started\n") { Foreground = Brushes.DarkOliveGreen, Background=Brushes.Yellow });
            }
            else if (e is LoggingStoppedEventArgs)
            {
                tbMonitor.Inlines.Add(new Run("Logging Completed\n") { Foreground = Brushes.Crimson, Background = Brushes.Yellow });
            }

            scrollMonitor.ScrollToBottom();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
