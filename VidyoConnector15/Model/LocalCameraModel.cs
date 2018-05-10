using VidyoClient;

namespace VidyoConnector.Model
{
    public class LocalCameraModel : DeviceModelBase
    {
        public LocalCameraModel(LocalCamera camera)
        {
            Object = camera;

            // if selected camera is 'NONE', then check this camera in 'Content Sharing' menu
            if (camera == null)
            {
                IsSharingContent = true;
            }
        }

        public LocalCamera Object { get; private set; }

        public string DisplayName
        {
            get { return Object == null ? "None" : Object.GetName(); }
        }

        public string Id
        {
            get { return Object == null ? null : Object.GetId(); }
        }

        private bool _isStreamingVideo;
        /// <summary>
        /// Indicates whether this camera is currently streaming video.
        /// </summary>
        public bool IsStreamingVideo
        {
            get { return _isStreamingVideo; }
            set
            {
                _isStreamingVideo = value;
                OnPropertyChanged();
                OnPropertyChanged("CanShareContent");
            }
        }

        private bool _isSharingContent;
        /// <summary>
        /// Indicates whether this camera is currently sharing video content.
        /// </summary>
        public bool IsSharingContent
        {
            get { return _isSharingContent; }
            set
            {
                _isSharingContent = value;
                OnPropertyChanged();
                OnPropertyChanged("CanStreamVideo");
            }
        }

        /// <summary>
        /// Indicates whether this camera is available for sharing video content.
        /// Camera is available to share content if it is not streaming video OR selected camera is 'NONE'.
        /// </summary>
        public bool CanShareContent { get { return !IsStreamingVideo || Object == null; } }

        /// <summary>
        /// Indicates whether this camera is available for video streaming.
        /// Camera is available to stream video if it is not sharing video content OR selected camera is 'NONE'.
        /// </summary>
        public bool CanStreamVideo { get { return !IsSharingContent || Object == null; } }
    }
}