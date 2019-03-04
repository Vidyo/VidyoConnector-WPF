using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using log4net;
using VidyoClient;
using VidyoConnector.Annotations;
using VidyoConnector.Commands;
using VidyoConnector.Listeners;
using VidyoConnector.Model;

namespace VidyoConnector.ViewModel
{
    /// <summary>
    /// Represents ViewModel object which operates application data.
    /// </summary>
    public class VidyoConnectorViewModel : INotifyPropertyChanged
    {
        private Connector _connector;

        // Used log4net here. Can be replaced with any other Logger.
        internal readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public VidyoConnectorViewModel()
        {
            LocalCameras = new ObservableCollection<LocalCameraModel>();
            LocalMicrophones = new ObservableCollection<LocalMicrophoneModel>();
            LocalSpeakers = new ObservableCollection<LocalSpeakerModel>();
            LocalWindows = new ObservableCollection<LocalWindowShareModel>();
            LocalMonistors = new ObservableCollection<LocalMonitorModel>();
            ChatMessages = new ObservableCollection<ChatMessageModel>();
            Host = @"prod.vidyo.io";
        }

        /// <summary>
        /// Entry point. Prepares and launches Vidyo functionality.
        /// </summary>
        /// <param name="handle">Control's handler where video will be rendered.</param>
        /// <param name="width">Width of video area.</param>
        /// <param name="height">Height of video area.</param>
        public void Init(IntPtr handle, uint width, uint height)
        {
            ConnectorPKG.Initialize();
            Log.Info("VidyoConnector initialized.");

            _connector = new Connector(handle, Connector.ConnectorViewStyle.ConnectorviewstyleDefault, 8, "all@VidyoClient", "VidyoClient.log", 0);

            // This should be called on each window resizing.
            _connector.ShowViewAtPoints(handle, 0, 0, width, height);
            Log.Info(string.Format("Showing view with width={0} and height={1}", width, height));

            // Adding Null's to devices collection, which is 'None' in GUI. Selecting 'None' means no device will be used.
            LocalCameras.Add(new LocalCameraModel(null));
            LocalMicrophones.Add(new LocalMicrophoneModel(null));
            LocalSpeakers.Add(new LocalSpeakerModel(null));

            // Registering to events we want to handle.
            _connector.RegisterLocalCameraEventListener(new LocalCameraListener(this));
            _connector.RegisterLocalWindowShareEventListener(new LocalWindowShareListener(this));
            _connector.RegisterLocalMicrophoneEventListener(new LocalMicropfoneListener(this));
            _connector.RegisterLocalSpeakerEventListener(new LocalSpeakerListener(this));
            _connector.RegisterParticipantEventListener(new ParticipantListener(this));
            _connector.RegisterLocalMonitorEventListener(new LocalMonitorListener(this));
            //_connector.RegisterLogEventListener(new LogListener(this), "*@VidyoClient");
            _connector.RegisterMessageEventListener(new MessageListener(this));

            // We are not in call when application started.
            ConnectionState = ConnectionState.NotConnected;

            // On application start Vidyo turns on default camera.
            IsCameraOn = true;
            // On application start Vidyo turns on default microphone.
            IsMicrophoneOn = true;
            EnableDebug = false;

            ParseCommandLineArgs();
        }

        /// <summary>
        /// Adjusts video panel size.
        /// </summary>
        /// <param name="handle">Control's handler where video will be rendered.</param>
        /// <param name="width">Width of video area.</param>
        /// <param name="height">Height of video area.</param>
        public void AdjustVideoPanelSize(IntPtr handle, uint width, uint height)
        {
            _connector.ShowViewAtPoints(handle, 0, 0, width, height);
        }

        #region Devices

        /*
         * This section contains collections of applications resources like:
         *  -   Local camera devices
         *  -   Local microphone devices
         *  -   Local speaker devices
         *  -   Local application windows which can be shared during conference
         *  -   Local monitors / screens which can be shared durin conference
         *  
         *  These properties contain a list of available devices which can be found in the application menu items.
         *  These properties are bindable on the UI.
         *  Each of them can be selected. Selecting 'None' device will tell the aaplication not to use device at all.
         */

        public ObservableCollection<LocalCameraModel> LocalCameras { get; set; }

        public ObservableCollection<LocalMicrophoneModel> LocalMicrophones { get; set; }

        public ObservableCollection<LocalSpeakerModel> LocalSpeakers { get; set; }

        public ObservableCollection<LocalWindowShareModel> LocalWindows { get; set; }

        public ObservableCollection<LocalMonitorModel> LocalMonistors { get; set; }

        #endregion

        #region LocalCamera

        /*
         * This section contains functionality for local camera devices control:
         *  -   Is local camera muted or not (bindable on UI)
         *  -   Adding / removing local camera device if such was plugged in / plugged out
         *  -   Selecting / deselecting specific camera on UI and on API level
         *  -   Muting / unmuting camera
         */

        private bool _isCameraOn;
        public bool IsCameraOn {
            get { return _isCameraOn; }
            set
            {
                _isCameraOn = value;
                Log.Info(string.Format("Set local camera is on={0}", value));
                OnPropertyChanged();
            }
        }

        public void AddLocalCamera(LocalCameraModel camera)
        {
            if (LocalCameras.FirstOrDefault(x => x.Id == camera.Id) == null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => LocalCameras.Add(camera));
                Log.Info(string.Format("Added local microphone: name={0} id={1}", camera.DisplayName, camera.Id));
            }
        }

        public void RemoveLocalCamera(LocalCameraModel camera)
        {
            var cameraToRemove = LocalCameras.FirstOrDefault(x => x.Id == camera.Id);
            if (cameraToRemove != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => LocalCameras.Remove(cameraToRemove));
                Log.Info(string.Format("Removed local camera: name={0} id={1}", cameraToRemove.DisplayName,
                    cameraToRemove.Id));
            }
        }

        public void SetSelectedLocalCamera(LocalCameraModel camera)
        {
            var cameraToSelect = LocalCameras.FirstOrDefault(x => x.Id == camera.Id);
            if (cameraToSelect != null)
            {
                LocalCameras.Select(x =>
                    {
                        x.IsStreamingVideo = false;
                        return x;
                    })
                    .ToList();
                cameraToSelect.IsStreamingVideo = true;

                Log.Info(string.Format("Local camera selected: name={0} id={1}", cameraToSelect.DisplayName,
                    cameraToSelect.Id));
            }
        }

        public void SetSelectedVideoContent(LocalCameraModel camera)
        {
            var camToSelect = LocalCameras.FirstOrDefault(x => x.Id == camera.Id);
            if (camToSelect != null)
            {
                LocalCameras.Select(x =>
                    {
                        x.IsSharingContent = false;
                        return x;
                    })
                    .ToList();
                camToSelect.IsSharingContent = true;

                Log.InfoFormat("Video content share selected: name={0} id={1}", camToSelect.DisplayName, camToSelect.IsSharingContent);
            }
        }

        public void SetSelectedLocalCamera(object cameraModelObj)
        {
            var camera = cameraModelObj as LocalCameraModel;
            if (camera != null)
            {
                SetSelectedLocalCamera(camera);
            }
        }

        public void SetSelectedVideoContent(object camerModelObj)
        {
            var camera = camerModelObj as LocalCameraModel;
            if (camera != null)
            {
                SetSelectedVideoContent(camera);
            }
        }

        private void SelectLocalCamera()
        {
            var camToSelect = LocalCameras.FirstOrDefault(x => x.IsStreamingVideo);
            if (camToSelect != null)
            {
                _connector.SelectLocalCamera(camToSelect.Object);
                Log.Info(string.Format("Set selected local camera: name={0} id={1}", camToSelect.DisplayName,
                    camToSelect.Id));
            }
        }

        private void ShareVideoContent()
        {
            var contentToShare = LocalCameras.FirstOrDefault(x => x.IsSharingContent);
            if (contentToShare != null)
            {
                _connector.SelectVideoContentShare(contentToShare.Object);
                Log.InfoFormat("Set selected video content sharing: name={0} id={1}", contentToShare.DisplayName,
                    contentToShare.Id);
            }
        }

        private void SetLocalCameraPrivacy()
        {
            if (IsCameraOn)
            {
                _connector.SetCameraPrivacy(true);
                Log.Info("Local camera muted.");
                IsCameraOn = false;
            }
            else
            {
                _connector.SetCameraPrivacy(false);
                Log.Info("Local camera unmuted.");
                IsCameraOn = true;
            }
        }

        #endregion

        #region LocalMicrophones

        /*
         * This section contains functionality for local microphone devices control:
         *  -   Is local microphone muted or not (bindable on UI)
         *  -   Adding / removing local microphone device if such was plugged in / plugged out
         *  -   Selecting / deselecting specific microphone on UI and on API level
         *  -   Muting / unmuting microphone
         */

        private bool _isMicOn;
        public bool IsMicrophoneOn {
            get { return _isMicOn; }
            set
            {
                _isMicOn = value;
                Log.Info(string.Format("Set local microphone is on={0}", value));
                OnPropertyChanged();
            }
        }

        public void AddLocalMicrophone(LocalMicrophoneModel mic)
        {
            if (LocalMicrophones.FirstOrDefault(x => x.Id == mic.Id) == null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { LocalMicrophones.Add(mic); });
                Log.Info(string.Format("Added local microphone: name={0} id={1}", mic.DisplayName, mic.Id));
            }
        }

        public void RemoveLocalMicrophone(LocalMicrophoneModel mic)
        {
            var micToRemove = LocalMicrophones.FirstOrDefault(x => x.Id == mic.Id);
            if (micToRemove != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { LocalMicrophones.Remove(micToRemove); });
                Log.Info(string.Format("Removed local microphone: name={0} id={1}", micToRemove.DisplayName,
                    micToRemove.Id));
            }
        }

        public void SetSelectedLocalMicrophone(LocalMicrophoneModel mic)
        {
            var micToSelect = LocalMicrophones.FirstOrDefault(x => x.Id == mic.Id);
            if (micToSelect != null)
            {
                LocalMicrophones.Select(x =>
                    {
                        x.IsStreamingAudio = false;
                        return x;
                    })
                    .ToList();
                micToSelect.IsStreamingAudio = true;

                Log.Info(string.Format("Local microphone selected: name={0} id={1}", micToSelect.DisplayName,
                    micToSelect.Id));
            }
        }

        public void SetSelectedAudioContent(LocalMicrophoneModel mic)
        {
            var micToSelect = LocalMicrophones.FirstOrDefault(x => x.Id == mic.Id);
            if (micToSelect != null)
            {
                LocalMicrophones.Select(x =>
                    {
                        x.IsSharingContent = false;
                        return x;
                    })
                    .ToList();
                micToSelect.IsSharingContent = true;

                Log.InfoFormat("Audio content share selected: name={0} id={1}", micToSelect.DisplayName, micToSelect.IsSharingContent);
            }
        }

        public void SetSelectedLocalMicrophone(object micModelObj)
        {
            var mic = micModelObj as LocalMicrophoneModel;
            if (mic != null)
            {
                SetSelectedLocalMicrophone(mic);
            }
        }

        public void SetSelectedAudioContent(object micModelObj)
        {
            var mic = micModelObj as LocalMicrophoneModel;
            if (mic != null)
            {
                SetSelectedAudioContent(mic);
            }
        }

        private void SelectLocalMicrophone()
        {
            var micToSelect = LocalMicrophones.FirstOrDefault(x => x.IsStreamingAudio);
            if (micToSelect != null)
            {
                _connector.SelectLocalMicrophone(micToSelect.Object);
                Log.Info(string.Format("Set selected local microphone: name={0} id={1}", micToSelect.DisplayName,
                    micToSelect.Id));
            }
        }

        private void ShareAudioContent()
        {
            var contentToShare = LocalMicrophones.FirstOrDefault(x => x.IsSharingContent);
            if (contentToShare != null)
            {
                _connector.SelectAudioContentShare(contentToShare.Object);
                Log.InfoFormat("Set selected audio content sharing: name={0} id={1}", contentToShare.DisplayName,
                    contentToShare.Id);
            }
        }

        private void SetLocalMicPrivacy()
        {
            if (IsMicrophoneOn)
            {
                _connector.SetMicrophonePrivacy(true);
                Log.Info("Local microphone muted.");
                IsMicrophoneOn = false;
            }
            else
            {
                _connector.SetMicrophonePrivacy(false);
                Log.Info("Local microphone unmuted.");
                IsMicrophoneOn = true;
            }
        }

        #endregion

        #region LocalSpeaker

        /*
         * This section contains functionality for local speaker devices control:
         *  -   Adding / removing local speaker device if such was plugged in / plugged out
         *  -   Selecting / deselecting specific speaker on UI and on API level
         */

        public void AddLocalSpeaker(LocalSpeakerModel speaker)
        {
            if (LocalSpeakers.FirstOrDefault(x => x.Id == speaker.Id) == null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { LocalSpeakers.Add(speaker); });
                Log.Info(string.Format("Added local speaker: name={0} id={1}", speaker.DisplayName, speaker.Id));
            }
        }

        public void RemoveLocalSpeaker(LocalSpeakerModel speaker)
        {
            var speakerToRemove = LocalSpeakers.FirstOrDefault(x => x.Id == speaker.Id);
            if (speakerToRemove != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { LocalSpeakers.Remove(speakerToRemove); });
                Log.Info(string.Format("Removed local speaker: name={0} id={1}", speakerToRemove.DisplayName,
                    speakerToRemove.Id));
            }
        }

        public void SetSelectedLocalSpeaker(LocalSpeakerModel speaker)
        {
            var speakerToSelect = LocalSpeakers.FirstOrDefault(x => x.Id == speaker.Id);
            if (speakerToSelect != null)
            {
                LocalSpeakers.Select(x =>
                    {
                        x.IsSelected = false;
                        return x;
                    })
                    .ToList();
                speakerToSelect.IsSelected = true;

                Log.Info(string.Format("Local speaker selected: name={0} id={1}", speakerToSelect.DisplayName,
                    speakerToSelect.Id));
            }
        }

        public void SetSelectedLocalSpeaker(object speakerModelObj)
        {
            var speaker = speakerModelObj as LocalSpeakerModel;
            if (speaker != null)
            {
                SetSelectedLocalSpeaker(speaker);
            }
        }

        private void SelectLocalSpeaker()
        {
            var speakerToSelect = LocalSpeakers.FirstOrDefault(x => x.IsSelected);
            if (speakerToSelect != null)
            {
                _connector.SelectLocalSpeaker(speakerToSelect.Object);
                Log.Info(string.Format("Set selected local speaker: name={0} id={1}", speakerToSelect.DisplayName,
                    speakerToSelect.Id));
            }
        }

        #endregion

        #region LocalWindows

        /*
         * This section contains functionality for local application windows control:
         *  -   Adding / removing application windows if such has been opened / closed
         *  -   Selecting / deselecting specific windows share on UI and on API level
         *  -   Starting share (used in command binding)
         */

        public void AddLocalWindow(LocalWindowShareModel window)
        {
            if (LocalWindows.FirstOrDefault(x => x.Id.Equals(window.Id)) == null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => LocalWindows.Add(window));
                Log.Info(string.Format("Added local window: name={0} id={1}", window.DisplayName, window.Id));
            }
        }

        public void RemoveLocalWindow(LocalWindowShareModel window)
        {
            var winToRemove = LocalWindows.FirstOrDefault(x => x.Id.Equals(window.Id));
            if (winToRemove != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => LocalWindows.Remove(winToRemove));
                Log.Info(
                    string.Format("Removed local window: name={0} id={1}", winToRemove.DisplayName, winToRemove.Id));
            }
        }

        public void SetSelectedLocalWindow(LocalWindowShareModel window)
        {
            LocalWindows.Select(x =>
                {
                    x.IsSelected = false;
                    return x;
                })
                .ToList();

            var winToSelect = LocalWindows.FirstOrDefault(x => x.Id.Equals(window.Id));
            if (winToSelect != null)
            {
                winToSelect.IsSelected = true;
                Log.Info(string.Format("Local window selected: name={0} id={1}", winToSelect.DisplayName,
                    winToSelect.Id));
            }
        }

        public void SetSelectedLocalWindow(object winModelObj)
        {
            var win = winModelObj as LocalWindowShareModel;
            if (win != null)
            {
                SetSelectedLocalWindow(win);
            }
        }

        private void StartLocalWindowShare()
        {
            var winToShare = LocalWindows.FirstOrDefault(x => x.IsSelected);
            if (winToShare != null)
            {
                SharingInProgress = _connector.SelectLocalWindowShare(winToShare.Object);
                Log.Info(string.Format("Set selected local window share: name={0} id={1}", winToShare.DisplayName,
                    winToShare.Id));
            }
        }

        #endregion

        #region LocalMonitors

        /*
         * This section contains functionality for local monitors (sreens) control:
         *  -   Adding / removing monitor if such has been plugged in / plugged out
         *  -   Selecting / deselecting specific monitor share on UI and on API level
         *  -   Starting share (used in command binding)
         */

        public void AddLocalMonitor(LocalMonitorModel monitor)
        {
            if (LocalMonistors.FirstOrDefault(x => x.Id.Equals(monitor.Id)) == null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => LocalMonistors.Add(monitor));
                Log.Info(string.Format("Added local monitor: name={0} id={1}", monitor.DisplayName, monitor.Id));
            }
        }

        public void RemoveLocalMonitor(LocalMonitorModel monitor)
        {
            var monitorToRemove = LocalMonistors.FirstOrDefault(x => x.Id.Equals(monitor.Id));
            if (monitorToRemove != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => LocalMonistors.Remove(monitorToRemove));
                Log.Info(string.Format("Removed local monitor: name={0} id={1}", monitorToRemove.DisplayName,
                    monitorToRemove.Id));
            }
        }

        public void SetSelectedLocalMonitor(LocalMonitorModel monitor)
        {
            LocalMonistors.Select(x =>
                {
                    x.IsSelected = false;
                    return x;
                })
                .ToList();

            var monitorToSelect = LocalMonistors.FirstOrDefault(x => x.Id.Equals(monitor.Id));
            if (monitorToSelect != null)
            {
                monitorToSelect.IsSelected = true;
                Log.Info(string.Format("Local window selected: name={0} id={1}", monitorToSelect.DisplayName,
                    monitorToSelect.Id));
            }
        }

        public void SetSelectedLocalMonitor(object monitorModelObj)
        {
            var monitor = monitorModelObj as LocalMonitorModel;
            if (monitor != null)
            {
                SetSelectedLocalMonitor(monitor);
            }
        }

        private void StartLocalMonitorShare()
        {
            var monitorToShare = LocalMonistors.FirstOrDefault(x => x.IsSelected);
            if (monitorToShare != null)
            {
                SharingInProgress = _connector.SelectLocalMonitor(monitorToShare.Object);
                Log.Info(string.Format("Set selected local window share: name={0} id={1}", monitorToShare.DisplayName,
                    monitorToShare.Id));
            }
        }

        #endregion

        #region General

        /*
         * This section contains general application functionality like:
         *  -   User info
         *  -   Conference state and error messages
         *  -   Participants activity messages
         *  -   Join / leave call (used in command binding)
         *  -   Shutting application down logic
         *  
         *  Properties are bindable on the UI
         */

        private string _host;
        public string Host {
            get { return _host; }
            set { _host = value; OnPropertyChanged(); }
        }

        private string _token;
        public string Token {
            get { return _token; }
            set { _token = value; OnPropertyChanged(); }
        }

        private string _displayName;
        public string DisplayName {
            get { return _displayName; }
            set { _displayName = value ?? string.Empty; OnPropertyChanged(); }
        }

        private string _roomId;
        public string RoomId {
            get { return _roomId; }
            set { _roomId = value; OnPropertyChanged(); }
        }

        private string _status;
        public string Status {
            get { return _status; }
            set
            {
                _status = value;
                Log.Info(string.Format("Set status={0}", value));
                OnPropertyChanged();
            }
        }

        private string _chatMessage;
        public string ChatMessage {
            get  { return _chatMessage; }
            set
            {
                _chatMessage = value;
                OnPropertyChanged();
            }
        }

        private string _participantsActivityLog;
        public string ParticipantsActivityLog {
            get { return _participantsActivityLog; }
            set
            {
                _participantsActivityLog = value;
                Log.Info(string.Format("Set participant activity={0}", value));
                OnPropertyChanged();
            }
        }

        private ConnectionState _connectionState;
        public ConnectionState ConnectionState {
            get { return _connectionState; }
            set
            {
                _connectionState = value;
                Log.Info(string.Format("Set conference connection state={0}", value));
                switch (value)
                {
                    case ConnectionState.Undefined:
                    case ConnectionState.NotConnected:
                        Status = "Ready to connect";
                        break;
                    case ConnectionState.Connected:
                        Status = "Connected";
                        break;
                    case ConnectionState.OperationInProgress:
                        Status = "Please wait...";
                        break;
                }
                OnPropertyChanged();
            }
        }

        private bool _enableDebug;
        public bool EnableDebug {
            get { return _enableDebug; }
            set
            {
                _enableDebug = value;
                Log.InfoFormat("Set Debug mode={0}", value);
                OnPropertyChanged();

                CommandToggleDebug.Execute(null);
            }
        }

        private string _error;
        public string Error {
            get { return _error; }
            set
            {
                _error = value;
                if (string.IsNullOrEmpty(value))
                    Log.Info("Error message cleared.");
                else 
                    Log.Warn(string.Format("Set error string={0}", value));
                OnPropertyChanged();
            }
        }

        private bool _sharingInProgress;
        public bool SharingInProgress {
            get { return _sharingInProgress; }
            set
            {
                _sharingInProgress = value;
                Log.Info(string.Format("Set sharing in progress={0}", value));
                OnPropertyChanged();
            }
        }

        private void StopSharing()
        {
            _connector.SelectLocalWindowShare(null);
            Log.Info("Local windows sharing stopped.");

            _connector.SelectLocalMonitor(null);
            Log.Info("Local monitors sharing stopped.");

            SharingInProgress = false;
        }

        private void JoinLeaveCall()
        {
            Log.Info(string.Format("Toggling join / leave call. Current state conference state={0}", ConnectionState));
            switch (ConnectionState)
            {
                case ConnectionState.NotConnected:
                    ConnectionState = ConnectionState.OperationInProgress;
                    Log.Info("Joining call...");
                    JoinCall();
                    break;
                case ConnectionState.Connected:
                case ConnectionState.OperationInProgress:
                    ConnectionState = ConnectionState.OperationInProgress;
                    Log.Info("Leaving call...");
                    LeaveCall();
                    break;
            }
        }

        private void JoinCall()
        {
            Error = null;
            try
            {
                var res = _connector.Connect(Host, Token, DisplayName, RoomId, new ConnectionListener(this));
                Log.DebugFormat("Returned '{0}'", res);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Join call failed. Reason: {0}", ex.Message);
            }

            Log.Info("Attempted to connect.");
        }

        private void LeaveCall()
        {
            _connector.Disconnect();
            Log.Info("Attempted to disconnect.");

            ConnectionState = ConnectionState.NotConnected;
        }

        private void QuitApplication()
        {
            var dlgRes = MessageBox.Show("Are you sure you want to close VidyoConnector?", "Exit",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dlgRes == MessageBoxResult.Yes)
            {
                Log.Info("Exiting the app... ");
                if (ConnectionState == ConnectionState.Connected)
                {
                    LeaveCall();
                }

                // Unregistering to events we want to handle.
                _connector.UnregisterLocalCameraEventListener();
                _connector.UnregisterLocalWindowShareEventListener();
                _connector.UnregisterLocalMicrophoneEventListener();
                _connector.UnregisterLocalSpeakerEventListener();
                _connector.UnregisterParticipantEventListener();
                _connector.UnregisterLocalMonitorEventListener();

                _connector.Disable();
                _connector = null;

                Log.Info("VidyoConnector disabled.");

                ConnectorPKG.Uninitialize();
                Log.Info("VidyoConnector unitialized.");

                System.Windows.Application.Current.Shutdown();
            }
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "VidyoClient-WinSDK Version " + _connector.GetVersion() + "\r\n\r\nCopyright © 2017-2018 Vidyo, Inc. All rights reserved.",
                "About VidyoConnector", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ToggleDebug()
        {
            if (EnableDebug)
            {
                _connector.EnableDebug(7776, "info@VidyoClient info@VidyoConnector warning");
                Log.Info("Debug mode disabled.");
            }
            else
            {
                _connector.DisableDebug();
                Log.Info("Debug mode enabled");
            }
        }

        private void SendChatMessage()
        {
            if (ConnectionState == ConnectionState.Connected)
            {
                _connector.SendChatMessage(ChatMessage);
                AddChatMessage("Me", ChatMessage);
                ChatMessage = string.Empty;
            }
        }

        private void ParseCommandLineArgs()
        {
            Log.Info("Parsing command line arguments...");
            var args = Environment.GetCommandLineArgs();
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var split = args[i].Split("=".ToCharArray(), 2);

                    if (split[0].Contains("host")) Host = split[1];
                    else if (split[0].Contains("token")) Token = split[1];
                    else if (split[0].Contains("displayName")) DisplayName = split[1];
                    else if (split[0].Contains("recourceId")) RoomId = split[1];
                    else if (split[0].Contains("hideConfig")) Log.Warn("hideConfig property is not implemented yet.");
                    else if (split[0].Contains("autoJoin")) Log.Warn("autoJoin property is not implemented yet.");
                    else if (split[0].Contains("allowReconnect")) Log.Warn("allowReconnect property is not implemented yet.");
                    else if (split[0].Contains("enableDebug"))
                    {
                        if (split[1].Equals("0")) EnableDebug = false;
                        else if (split[1].Equals("1")) EnableDebug = true;
                    }
                    else if (split[0].Contains("returnURL")) Log.Warn("returnURL property is not implemented yet.");
                    else Log.ErrorFormat("Argument {0} is not valid.", split[0]);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error while parsing command line arguments: {0}", ex.Message);
            }
        }

        #endregion

        #region Chat

        public ObservableCollection<ChatMessageModel> ChatMessages { get; set; }

        public void AddChatMessage(string senderName, string messageBody)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                ChatMessages.Add(new ChatMessageModel { Sender = senderName, Message = messageBody })); 
            
        }

        #endregion

        #region Commands

        private ICommand _commandJoinLeaveCall;
        public ICommand CommandJoinLeaveCall
        {
            get { return GetCommand(ref _commandJoinLeaveCall, x => JoinLeaveCall()); }
        }

        private ICommand _commandSelectLocalCamera;
        public ICommand CommandSelectLocalCamera {
            get { return GetCommand(ref _commandSelectLocalCamera, x => SelectLocalCamera()); }
        }

        private ICommand _commandShareVideoContent;
        public ICommand CommandShareVideoContent {
            get { return GetCommand(ref _commandShareVideoContent, x => ShareVideoContent()); }
        }

        private ICommand _commandSelectLocalMic;
        public ICommand CommandSelectLocalMicrophone {
            get { return GetCommand(ref _commandSelectLocalMic, x => SelectLocalMicrophone()); }
        }

        private ICommand _commandShareAudioContent;
        public ICommand CommandShareAudioContent {
            get { return GetCommand(ref _commandShareAudioContent, x => ShareAudioContent()); }
        }

        private ICommand _commandSelectLocalSpeaker;
        public ICommand CommandSelectLocalSpeaker {
            get { return GetCommand(ref _commandSelectLocalSpeaker, x => SelectLocalSpeaker()); }
        }

        private ICommand _commandSetLocalCameraPrivacy;
        public ICommand CommandSetLocalCameraPrivacy {
            get { return GetCommand(ref _commandSetLocalCameraPrivacy, x => SetLocalCameraPrivacy()); }
        }

        private ICommand _commandSetLocalMicPrivacyCommand;
        public ICommand CommandSetLocalMicrophonePrivacy {
            get { return GetCommand(ref _commandSetLocalMicPrivacyCommand, x => SetLocalMicPrivacy()); }
        }

        private ICommand _commandStartShareWindow;
        public ICommand CommandStartShareWindow {
            get { return GetCommand(ref _commandStartShareWindow, x => StartLocalWindowShare()); }
        }

        private ICommand _commandStartShareMonitor;
        public ICommand CommandStartShareMonitor {
            get { return GetCommand(ref _commandStartShareMonitor, x => StartLocalMonitorShare()); }
        }

        private ICommand _commandStopSharing;
        public ICommand CommandStopSharing {
            get { return GetCommand(ref _commandStopSharing, x => StopSharing()); }
        }

        private ICommand _commandQuitApp;
        public ICommand CommandQuitApplication { get { return GetCommand(ref _commandQuitApp, x => QuitApplication()); } }

        private ICommand _commandAbout;
        public ICommand CommandAbout {
            get { return GetCommand(ref _commandAbout, x => ShowAbout()); }
        }

        private ICommand _commandToggleDebug;
        public ICommand CommandToggleDebug { get { return GetCommand(ref _commandToggleDebug, x => ToggleDebug()); } }

        private ICommand _commandSendChatMessage;
        public ICommand CommandSendChatMessage { get { return GetCommand(ref _commandSendChatMessage, x => SendChatMessage()); } }

        private static ICommand GetCommand(ref ICommand command, Action<object> action, bool isCanExecute = true)
        {
            if (command != null) return command;

            var cmd = new BindableCommand { IsCanExecute = isCanExecute };
            cmd.ExecuteAction += action;
            command = cmd;

            return command;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}