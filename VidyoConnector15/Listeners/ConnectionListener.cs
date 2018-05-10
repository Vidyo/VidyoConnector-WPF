using System;
using System.Reflection;
using VidyoClient;
using VidyoConnector.Model;
using VidyoConnector.ViewModel;

namespace VidyoConnector.Listeners
{
    public class ConnectionListener : ListenerBase, Connector.IConnect
    {
        public ConnectionListener(VidyoConnectorViewModel viewModel) : base(viewModel) { }

        public void OnSuccess()
        {
            LogCallback(MethodBase.GetCurrentMethod().Name);
            ViewModel.ConnectionState = ConnectionState.Connected;
        }

        public void OnFailure(Connector.ConnectorFailReason reason)
        {
            LogCallback(MethodBase.GetCurrentMethod().Name);
            ViewModel.ConnectionState = ConnectionState.NotConnected;
            ViewModel.Error = string.Format("Connection failed {0}Reason: {1}", Environment.NewLine, reason);
        }

        public void OnDisconnected(Connector.ConnectorDisconnectReason reason)
        {
            LogCallback(MethodBase.GetCurrentMethod().Name);
            ViewModel.ConnectionState = ConnectionState.NotConnected;
        }
    }
}