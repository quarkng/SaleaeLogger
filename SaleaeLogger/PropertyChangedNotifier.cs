using System.ComponentModel;

namespace SaleaeLogger
{
    public class PropertyChangedNotifier : INotifyPropertyChanged
    {
        //================================================================================
        #region Stuff for INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
        public void OnPropertyChanged(string propName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propName));
        }
        public void OnPropertyChanged()
        {
            OnPropertyChanged(new PropertyChangedEventArgs(null));
        }
        #endregion
    }
}
