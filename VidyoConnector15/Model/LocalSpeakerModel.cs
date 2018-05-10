using VidyoClient;

namespace VidyoConnector.Model
{
    public class LocalSpeakerModel : DeviceModelBase
    {
        public LocalSpeakerModel(LocalSpeaker speaker)
        {
            Object = speaker;
        }

        public LocalSpeaker Object { get; private set; }

        public string DisplayName
        {
            get { return Object == null ? "None" : Object.GetName(); }
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