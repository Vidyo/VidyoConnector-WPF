using VidyoClient;
using VidyoConnector.Model;
using VidyoConnector.ViewModel;

namespace VidyoConnector.Listeners
{
    public class LocalCameraListener : ListenerBase, Connector.IRegisterLocalCameraEventListener
    {
        public LocalCameraListener(VidyoConnectorViewModel viewModel) : base(viewModel) { }

        public void OnLocalCameraAdded(LocalCamera localCamera)
        {
            ViewModel.AddLocalCamera(new LocalCameraModel(localCamera));
        }

        public void OnLocalCameraRemoved(LocalCamera localCamera)
        {
            ViewModel.RemoveLocalCamera(new LocalCameraModel(localCamera));
        }

        public void OnLocalCameraSelected(LocalCamera localCamera)
        {
            ViewModel.SetSelectedLocalCamera(new LocalCameraModel(localCamera));
        }

        public void OnLocalCameraStateUpdated(LocalCamera localCamera, Device.DeviceState state)
        {

        }

        
    }
}