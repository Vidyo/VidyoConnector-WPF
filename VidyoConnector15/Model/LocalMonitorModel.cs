using VidyoClient;

namespace VidyoConnector.Model
{
    public class LocalMonitorModel : DeviceModelBase
    {
        public LocalMonitorModel(LocalMonitor monitor)
        {
            Object = monitor;
        }

        public LocalMonitor Object { get; private set; }

        public string DisplayName
        {
            get { return Object == null ? null : Object.GetName(); }
        }

        public string Id
        {
            get { return Object == null ? null : Object.GetId(); }
        }

        private bool _isSelected;
        /// <summary>
        /// Indicates whether specific resource is selected for using.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }
}