using VidyoClient;
using VidyoConnector.ViewModel;

namespace VidyoConnector.Listeners
{
    public class MessageListener : ListenerBase, Connector.IRegisterMessageEventListener
    {
        public MessageListener(VidyoConnectorViewModel viewModel) : base(viewModel) { }

        public void OnChatMessageReceived(Participant participant, ChatMessage chatMessage)
        {
            if (participant != null && chatMessage != null)
            {
                ViewModel.AddChatMessage(participant.GetName(), chatMessage.body);
            }
        }
    }
}
