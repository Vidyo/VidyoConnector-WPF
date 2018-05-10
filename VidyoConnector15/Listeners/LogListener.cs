using VidyoClient;
using VidyoConnector.ViewModel;

namespace VidyoConnector.Listeners
{
    public class LogListener : ListenerBase, Connector.IRegisterLogEventListener
    {
        public LogListener(VidyoConnectorViewModel viewModel) : base(viewModel) { }

        public void OnLog(LogRecord logRecord)
        {
            ViewModel.Log.InfoFormat("{0} | {1} | {2} | {3} | {4} | {5}", logRecord.level, logRecord.file, logRecord.line, logRecord.functionName, logRecord.name, logRecord.message);
        }
    }
}