using VidyoClient;
using VidyoConnector.Model;
using VidyoConnector.ViewModel;

namespace VidyoConnector.Listeners
{
    public class LocalSpeakerListener : ListenerBase, Connector.IRegisterLocalSpeakerEventListener
    {
        public LocalSpeakerListener(VidyoConnectorViewModel viewModel) : base(viewModel) { }

        public void OnLocalSpeakerAdded(LocalSpeaker localSpeaker)
        {
            ViewModel.AddLocalSpeaker(new LocalSpeakerModel(localSpeaker));
        }

        public void OnLocalSpeakerRemoved(LocalSpeaker localSpeaker)
        {
            ViewModel.RemoveLocalSpeaker(new LocalSpeakerModel(localSpeaker));
        }

        public void OnLocalSpeakerSelected(LocalSpeaker localSpeaker)
        {
            ViewModel.SetSelectedLocalSpeaker(new LocalSpeakerModel(localSpeaker));
        }

        public void OnLocalSpeakerStateUpdated(LocalSpeaker localSpeaker, Device.DeviceState state)
        {
            
        }
    }
}