#region license
/*
    The MIT License (MIT)
    Copyright (c) 2019 Techfactory.hu
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OBSMidiRemote.Lib
{
    public interface IOBSConnector
    {

        #region EventHandlers

        event OBSConnectionStatus ConnectionStatus;
        event OBSSceneListChanged SceneListChanged;
        event OBSSceneChanged SceneChanged;
        event OBSSceneItemChanged SceneItemChanged;
        event OBSStreamingStatusChanged StreamingStatusChanged;
        event OBSSourceChanged SourceChanged;
        event OBSAudioSourceChanged AudioSourceChanged;
        event OBSTransitionChanged TransitionChanged;
        event OBSStudioModeChanged ModeChanged;
        #endregion

        SOBSStreamingState State { get; }
        SOBSScene ActiveScene { get; }
        SOBSScene ActivePreviewScene { get; }
        SOBSTransition ActiveTransition { get; }

        List<SOBSScene> Scenes { get; }
        List<SOBSScene> PreviewScenes { get; }
        List<SOBSTransition> Transitions { get; }
        List<SOBSSource> Sources { get; }
        List<SOBSAudioSource> AudioSources { get;}
        EOBSMode Mode { get; }
        bool Connected { get; }

        void Connect(string url, int port, string token = null);
        void Disconnect();
        Task<bool> Reload();
        void CheckTimeouts();

        void SwitchScene(int id);
        void SwitchScene(string name);

        void SwitchPreviewScene(int id);
        void SwitchPreviewScene(string name);

        void SetSceneItemVisibility(string scene_name, string item_name, bool visible);
        void SetSceneItemVisibility(int scene_id, int item_id, bool visible);
        void ToggleSceneItemVisibility(int scene_id, int item_id);

        void SetSourceAudioMute(int source_id, bool muted);
        void ToggleSourceAudioMute(int source_id);

        void StartStreaming();
        void StopStreaming();
        void ToggleStreaming();
        void StartRecording();
        void StopRecording();
        void ToggleRecording();
        void StartReplayBuffer();
        void StopReplayBuffer();
        void SaveReplayBuffer();
        void ToggleReplayBuffer();

        void SetStudioMode(bool mode);
        void ToggleStudioMode();
        void SetTransition(int tr_id);

        void SetSourceAudioVolume(int source_id, float volume);

        string GetVersionInfo();
    }
}
