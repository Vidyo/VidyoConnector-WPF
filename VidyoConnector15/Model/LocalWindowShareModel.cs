using VidyoClient;

namespace VidyoConnector.Model
{
    public class LocalWindowShareModel : DeviceModelBase
    {
        public LocalWindowShareModel(LocalWindowShare window)
        {
            Object = window;
        }

        public LocalWindowShare Object { get; private set; }

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