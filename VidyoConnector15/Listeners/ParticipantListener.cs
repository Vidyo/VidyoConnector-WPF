using System;
using System.Collections.Generic;
using VidyoClient;
using VidyoConnector.ViewModel;

namespace VidyoConnector.Listeners
{
    public class ParticipantListener : ListenerBase, Connector.IRegisterParticipantEventListener
    {
        public ParticipantListener(VidyoConnectorViewModel viewModel) : base(viewModel) { }

        public void OnParticipantJoined(Participant participant)
        {
            var name = participant.GetName();
            if (!string.IsNullOrEmpty(name))
            {
                ViewModel.ParticipantsActivityLog = string.Format("{0} joined", name);
            }
        }

        public void OnParticipantLeft(Participant participant)
        {
            var name = participant.GetName();
            if (!string.IsNullOrEmpty(name))
            {
                ViewModel.ParticipantsActivityLog = string.Format("{0} has left", name);
            }
        }

        public void OnDynamicParticipantChanged(List<Participant> participants)
        {
            
        }

        public void OnLoudestParticipantChanged(Participant participant, bool audioOnly)
        {
            var name = participant.GetName();
            if (!string.IsNullOrEmpty(name))
            {
                ViewModel.ParticipantsActivityLog = string.Format("{0} speaking...", name);
            }
        }
    }
}
