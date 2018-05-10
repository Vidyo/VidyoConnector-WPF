namespace VidyoConnector.Model
{
    /// <summary>
    /// Indicates Conference / Connection state
    /// </summary>
    public enum ConnectionState
    {
        Undefined = 0,

        /// <summary>
        /// In conference
        /// </summary>
        Connected,
        
        /// <summary>
        /// Join or leaving the conference
        /// </summary>
        OperationInProgress,

        /// <summary>
        /// Not in conference
        /// </summary>
        NotConnected
    }
}