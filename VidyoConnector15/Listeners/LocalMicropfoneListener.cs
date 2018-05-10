using VidyoClient;
using VidyoConnector.Model;
using VidyoConnector.ViewModel;

namespace VidyoConnector.Listeners
{
    public class LocalMicropfoneListener : ListenerBase, Connector.IRegisterLocalMicrophoneEventListener
    {
        public LocalMicropfoneListener(VidyoConnectorViewModel viewModel) : base(viewModel) { }

        public void OnLocalMicrophoneAdded(LocalMicrophone localMicrophone)
        {
            ViewModel.AddLocalMicrophone(new  LocalMicrophoneModel(localMicrophone));
        }

        public void OnLocalMicrophoneRemoved(LocalMicrophone localMicrophone)
        {
            ViewModel.RemoveLocalMicrophone(new LocalMicrophoneModel(localMicrophone));
        }

        public void OnLocalMicrophoneSelected(LocalMicrophone localMicrophone)
        {
            ViewModel.SetSelectedLocalMicrophone(new LocalMicrophoneModel(localMicrophone));
        }

        public void OnLocalMicrophoneStateUpdated(LocalMicrophone localMicrophone, Device.DeviceState state)
        {
            
        }
    }
}