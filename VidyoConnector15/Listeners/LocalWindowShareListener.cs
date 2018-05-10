using VidyoClient;
using VidyoConnector.Model;
using VidyoConnector.ViewModel;

namespace VidyoConnector.Listeners
{
    public class LocalWindowShareListener : ListenerBase, Connector.IRegisterLocalWindowShareEventListener
    {
        public LocalWindowShareListener(VidyoConnectorViewModel viewModel) : base(viewModel) { }
        public void OnLocalWindowShareAdded(LocalWindowShare localWindowShare)
        {
            if (!string.IsNullOrEmpty(localWindowShare.GetName()))
            {
                ViewModel.AddLocalWindow(new LocalWindowShareModel(localWindowShare));
            }
        }

        public void OnLocalWindowShareRemoved(LocalWindowShare localWindowShare)
        {
            ViewModel.RemoveLocalWindow(new LocalWindowShareModel(localWindowShare));
        }

        public void OnLocalWindowShareSelected(LocalWindowShare localWindowShare)
        {
            ViewModel.SetSelectedLocalWindow(new LocalWindowShareModel(localWindowShare));
        }

        public void OnLocalWindowShareStateUpdated(LocalWindowShare localWindowShare, Device.DeviceState state)
        {
            
        }

        
    }
}