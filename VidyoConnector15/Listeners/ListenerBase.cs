using System;
using VidyoConnector.ViewModel;

namespace VidyoConnector.Listeners
{
    /// <summary>
    /// Represents common members of all Listeners.
    /// </summary>
    public class ListenerBase
    {
        /// <summary>
        /// ViewModel object which operates application data.
        /// </summary>
        protected readonly VidyoConnectorViewModel ViewModel;

        public ListenerBase(VidyoConnectorViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public void LogCallback(string name)
        {
            ViewModel.Log.Debug(string.Format("Recieved callback: {0}", name));
        }
    }
}