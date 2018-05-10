using VidyoClient;
using VidyoConnector.Model;
using VidyoConnector.ViewModel;

namespace VidyoConnector.Listeners
{
    public class LocalMonitorListener : ListenerBase, Connector.IRegisterLocalMonitorEventListener
    {
        public LocalMonitorListener(VidyoConnectorViewModel viewModel) : base(viewModel) { }

        public void OnLocalMonitorAdded(LocalMonitor localMonitor)
        {
            if (!string.IsNullOrEmpty(localMonitor.GetName()))
            {
                ViewModel.AddLocalMonitor(new LocalMonitorModel(localMonitor));
            }
        }

        public void OnLocalMonitorRemoved(LocalMonitor localMonitor)
        {
            ViewModel.RemoveLocalMonitor(new LocalMonitorModel(localMonitor));
        }

        public void OnLocalMonitorSelected(LocalMonitor localMonitor)
        {
            ViewModel.SetSelectedLocalMonitor(new LocalMonitorModel(localMonitor));
        }

        public void OnLocalMonitorStateUpdated(LocalMonitor localMonitor, Device.DeviceState state)
        {
            
        }
    }
}