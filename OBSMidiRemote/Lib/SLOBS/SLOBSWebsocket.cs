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
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace OBSMidiRemote.Lib.SLOBS
{
    public class SLOBSWebsocket
    {
        #region EventHandlers

        public event SLOBSConnectionStatus ConnectionStatus;
        public event SLOBSSceneListChanged SceneListChanged;

        public event SLOBSSceneChanged SceneChanged;
        public event SLOBSSceneItemChanged SceneItemChanged;
        public event SLOBSStreamingStatusChanged StreamingStatusChanged;
        public event SLOBSSourceChanged SourceChanged;

        #endregion

        private Dictionary<int, TaskCompletionSource<JObject>> _responseHandlers;
        private Dictionary<string, string> _subscribedEvents = new Dictionary<string, string>();
        private Dictionary<int, SLOBSAsyncCallback> _asyncresponseHandlers;
        private bool _asyncCallInprogress = false;
        private int _msgid = 0;

        public bool Connected { get; private set; }

        public WebSocket WSConnection { get; private set; }

        public SLOBSWebsocket()
        {
            _asyncresponseHandlers = new Dictionary<int, SLOBSAsyncCallback>();
            _responseHandlers = new Dictionary<int, TaskCompletionSource<JObject>>();
        }

        public void Connect(string url)
        {
            if (WSConnection != null && WSConnection.IsAlive)
                Disconnect();
            try
            {
                WSConnection = new WebSocket(url);
                WSConnection.WaitTime = TimeSpan.FromSeconds(2);
                WSConnection.OnMessage += WebsocketMessageHandler;
              
                WSConnection.OnClose += (s, e) =>
                {
                    Connected = false;
                    _connectionStatus(ESLOBSConnectionState.Disconnected, e.Reason);
                };
                WSConnection.OnError += (s, e) =>
                {
                    _connectionStatus(ESLOBSConnectionState.Error, e.Message);
                };
                WSConnection.OnOpen += (s, e) =>
                {
                    Connected = true;
                    _connectionStatus(ESLOBSConnectionState.Connected);
                };
                WSConnection.ConnectAsync();
                

            }
            catch (Exception e)
            {
                _connectionStatus(ESLOBSConnectionState.Error, e.Message);
            }
        }

        public void Disconnect()
        {
            if (WSConnection != null)
                WSConnection.Close();

            WSConnection = null;

            Connected = false;

            JObject err = new JObject();
            err.Add("status", "error");
            err.Add("error", "Disconnected");
            foreach (var acb in _asyncresponseHandlers)
            {
                acb.Value.callable.Invoke(err);
            }
            _connectionStatus(ESLOBSConnectionState.Disconnected);
        }

        public void CheckTimeouts()
        {
            if (_asyncresponseHandlers.Count > 0)
            {
                foreach (var acb in _asyncresponseHandlers)
                {
                    if (((TimeSpan)(DateTime.Now - acb.Value.timestamp)).TotalMilliseconds > 5000)
                    {
                        JObject err = new JObject();
                        err.Add("status", "error");
                        err.Add("error", "Timeout");
                        acb.Value.callable.Invoke(err);
                        _asyncresponseHandlers.Remove(acb.Key);
                    }
                }
            }

            if (_asyncresponseHandlers.Count == 0 && _asyncCallInprogress)
            {
                _asyncCallInprogress = false;
                //trigger event
               // if (AsyncQueueEmpty != null) { AsyncQueueEmpty(this, new EventArgs()); }
            }
        }

        public async Task<List<SLOBSScene>> ListScenes()
        {
            List<SLOBSScene> sceneList = new List<SLOBSScene>();
            var customParams = new JObject();
            customParams.Add("resource", "ScenesService");
            JObject result = await SendRequest("getScenes", customParams);
            if (result["error"] == null && result["result"] != null)
            {
                JArray scenes = (JArray)result["result"];
                foreach (JObject item in scenes)
                {
                    var sitem = new SLOBSScene(item);
                    sceneList.Add(sitem);
                }
            }
            return sceneList;
        }

        public async Task<List<SLOBSAudioSource>> ListAudioSources()
        {
            List<SLOBSAudioSource> audioSources = new List<SLOBSAudioSource>();
            var customParams = new JObject();
            customParams.Add("resource", "AudioService");
            JObject result = await SendRequest("getSources", customParams);
            if (result["error"] == null && result["result"] != null)
            {
                JArray scenes = (JArray)result["result"];
                foreach (JObject item in scenes)
                {
                    audioSources.Add(new SLOBSAudioSource(item));
                }
            }
            return audioSources;
        }

        public async Task<List<SLOBSSource>> ListSources()
        {
            List<SLOBSSource> sources = new List<SLOBSSource>();
            var customParams = new JObject();
            customParams.Add("resource", "SourcesService");
            JObject result = await SendRequest("getSources", customParams);
            if (result["error"] == null && result["result"] != null)
            {
                JArray scenes = (JArray)result["result"];
                foreach (JObject item in scenes)
                {
                    sources.Add(new SLOBSSource(item));
                }
            }
            return sources;
        }

        public async Task<SLOBSStreamingState?> GetStreamingStatus()
        {
            List<SLOBSAudioSource> audioSources = new List<SLOBSAudioSource>();
            var customParams = new JObject();
            customParams.Add("resource", "StreamingService");
            JObject result = await SendRequest("getModel", customParams);

            if (result["error"] == null && result["result"] != null)
            {
                var status = new SLOBSStreamingState();
                status.StreamingStatus = _streamingStateFromString((string)result["result"]["streamingStatus"]);
                status.RecordingStatus = _recordingStateFromString((string)result["result"]["recordingStatus"]);
                status.ReplayBufferStatus = _replayBufferStateFromString((string)result["result"]["replayBufferStatus"]);

                return status;
            }

            return null;
        }

        public async Task<SLOBSScene?> GetActiveScene()
        {
            var customParams = new JObject();
            customParams.Add("resource", "ScenesService");
            JObject result = await SendRequest("activeScene", customParams);
            if (result["error"] == null && result["result"] != null)
            {
                return new SLOBSScene((JObject)result["result"]);
            }
            return null;
        }

        public async Task<bool> SubscribeToEvent(string eventname, string resource)
        {
            if (_subscribedEvents.ContainsKey(resource+"."+ eventname))
            {
                return true;
            }

            var customParams = new JObject();
            customParams.Add("resource", resource);
            JObject result = await SendRequest(eventname, customParams);
            if (result["error"] == null && result["result"] != null)
            {
                if (result["result"]["_type"] != null && (string)result["result"]["_type"] == "SUBSCRIPTION")
                {
                    _subscribedEvents.Add((string)result["result"]["resourceId"], (string)result["result"]["emitter"]);
                }
            }
            return false;
        }

        public async Task<bool> Auth(string token)
        {
            var tokenarg = new JArray();
            tokenarg.Add(token);
            var customParams = new JObject();
            customParams.Add("resource", "TcpServerService");
            customParams.Add("args", tokenarg);
            JObject result = await SendRequest("auth", customParams);
            if (result["error"] == null)
            {
                return (bool)result["result"];
            }
            return false;
        }

        public async Task<string> GetActiveSceneId()
        {
            var customParams = new JObject();
            customParams.Add("resource", "ScenesService");
            JObject result = await SendRequest("activeSceneId", customParams);
            if (result["error"] == null)
            {
                return (string)result["result"];
            }
            return null;
        }

        public async Task<string> GetSceneItemResourceId(string scene_resid, string item_id)
        {
            var customParams = new JObject();
            var args = new JArray();
            args.Add(item_id);
            customParams.Add("resource", scene_resid);
            customParams.Add("args", args);
            JObject result = await SendRequest("getItem", customParams);
            if (JTokenIsNullOrEmpty(result["error"]) && !JTokenIsNullOrEmpty(result["result"]))
            {
                return (string)result["result"]["resourceId"];
            }
            return null;
        }

        public void SwitchScene(string resourceId)
        {
            JObject customParams = new JObject();
            JObject param = new JObject();
            JArray args = new JArray();
            args.Add(true);
            param.Add("resource", resourceId);
            param.Add("args", args);
            customParams.Add("method", "makeActive");
            customParams.Add("params", param);
            SyncRequest(customParams);
        }

        public void ToggleStream()
        {
            JObject customParams = new JObject();
            JObject param = new JObject();
            param.Add("resource", "StreamingService");
            customParams.Add("method", "toggleStreaming");
            customParams.Add("params", param);
            SyncRequest(customParams);
        }

        public void ToggleRecord()
        {
            JObject customParams = new JObject();
            JObject param = new JObject();
            param.Add("resource", "StreamingService");
            customParams.Add("method", "toggleRecording");
            customParams.Add("params", param);
            SyncRequest(customParams);
        }

        public void StartReplayBuffer()
        {
            JObject customParams = new JObject();
            JObject param = new JObject();
            param.Add("resource", "StreamingService");
            customParams.Add("method", "startReplayBuffer");
            customParams.Add("params", param);
            SyncRequest(customParams);
        }

        public void StopReplayBuffer()
        {
            JObject customParams = new JObject();
            JObject param = new JObject();
            param.Add("resource", "StreamingService");
            customParams.Add("method", "stopReplayBuffer");
            customParams.Add("params", param);
            SyncRequest(customParams);
        }

        public void SaveReplay()
        {
            JObject customParams = new JObject();
            JObject param = new JObject();
            param.Add("resource", "StreamingService");
            customParams.Add("method", "saveReplay");
            customParams.Add("params", param);
            SyncRequest(customParams);
        }

        public void SetAudioSourceMuted(string resource_id, bool mute = true)
        {
            JObject customParams = new JObject();
            JObject param = new JObject();
            JArray args = new JArray();
            args.Add(mute);
            param.Add("resource", resource_id);
            param.Add("args", args);
            customParams.Add("method", "setMuted");
            customParams.Add("params", param);
            SyncRequest(customParams);
        }

        public void SetAudioSourceVolume(string resource_id, float volume = 1f)
        {
            JObject customParams = new JObject();
            JObject param = new JObject();
            JArray args = new JArray();
            args.Add(volume);
            param.Add("resource", resource_id);
            param.Add("args", args);
            customParams.Add("method", "setDeflection");
            customParams.Add("params", param);
            SyncRequest(customParams);
        }

        public void SetSceneItemVisibility(string resource_id, bool visible = true)
        {
            JObject customParams = new JObject();
            JObject param = new JObject();
            JArray args = new JArray();
            args.Add(visible);
            param.Add("resource", resource_id);
            param.Add("args", args);
            customParams.Add("method", "setVisibility");
            customParams.Add("params", param);
            SyncRequest(customParams);
        }

        public Task<JObject> Request(JObject parameters)
        {
            var tcs = new TaskCompletionSource<JObject>();
            int msgid = NextMessageId();

            var body = new JObject();
            body.Add("jsonrpc", "2.0");
            body.Add("id", msgid);
            body.Merge(parameters);

            string rq = body.ToString().Replace("\n", "");
            _responseHandlers.Add(msgid, tcs);
            WSConnection.SendAsync(rq, delegate (bool completed) { });
            return tcs.Task;
        }

        public Task<JObject> SendRequest(string method, JObject customParams = null)
        {
            var body = new JObject();
            body.Add("method", method);
            if (customParams != null)
            {
                body.Add("params", customParams);
            }
            return Request(body);
        }

        public void SyncRequest(JObject parameters)
        {
            int msgid = NextMessageId();
            var body = new JObject();
            body.Add("jsonrpc", "2.0");
            body.Add("id", msgid);
            body.Merge(parameters);

            string rq = body.ToString().Replace("\n", "");
            WSConnection.SendAsync(rq, delegate (bool completed) { });
        }

        public void SendRequestAsync(Action<JObject> callback, string method, JObject customParams = null)
        {
            int msgid = NextMessageId();

            var body = new JObject();
            body.Add("jsonrpc", "2.0");
            body.Add("id", msgid);
            body.Add("method", method);
            if (customParams != null)
            {
                body.Add("params", customParams);
            }
            _asyncresponseHandlers.Add(msgid, new SLOBSAsyncCallback(callback));
            _asyncCallInprogress = true;
            WSConnection.Send((body.ToString().Replace("\n","")));
        }

        private bool JTokenIsNullOrEmpty(JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }

        private ESLOBSStreamingState _streamingStateFromString(string str)
        {
            var nstr = str.ToUpper().Substring(0,1) +""+ str.ToLower().Substring(1);
            try
            {
                return (ESLOBSStreamingState)Enum.Parse(typeof(ESLOBSStreamingState), nstr);
            }catch(Exception e)
            {
                return ESLOBSStreamingState.Offline;
            }
        }

        private ESLOBSSRecordingState _recordingStateFromString(string str)
        {
            var nstr = str.ToUpper().Substring(0, 1) + "" + str.ToLower().Substring(1);
            try
            {
                return (ESLOBSSRecordingState)Enum.Parse(typeof(ESLOBSSRecordingState), nstr);
            }
            catch (Exception e)
            {
                return ESLOBSSRecordingState.Offline;
            }
        }

        private ESLOBSSReplayBufferState _replayBufferStateFromString(string str)
        {
            var nstr = str.ToUpper().Substring(0, 1) + "" + str.ToLower().Substring(1);
            try
            {
                return (ESLOBSSReplayBufferState)Enum.Parse(typeof(ESLOBSSReplayBufferState), nstr);
            }
            catch (Exception e)
            {
                return ESLOBSSReplayBufferState.Offline;
            }
        }

        private int NextMessageId()
        {
            _msgid++;
            if (_msgid>10000) { _msgid = 1; }
            return _msgid;
        }

        public void WebsocketMessageHandler(object sender, MessageEventArgs e)
        {
  
            JObject body = JObject.Parse(e.Data);
            if (!e.IsText)
            {
                return;
            }

            //response
            if ((string)body["id"] != null)
            {
                int msgID = (int)body["id"];

                //await msg handlers
                if (_responseHandlers.ContainsKey(msgID))
                {
                    var handler = _responseHandlers[msgID];
                    if (handler != null)
                    {
                        try
                        {
                            handler.SetResult(body);
                        }
                        catch (Exception ex)
                        {

                        }
                        _responseHandlers.Remove(msgID);
                    }
                }
            }
            //event
            else if (body["result"] != null)
            {
                if (body["result"]["_type"] != null && (string)body["result"]["_type"] == "EVENT")
                {
                    ProcessEvent(body);
                }
            }
        }

        private void ProcessEvent(JObject body)
        {
            
            if (body["result"] == null) { return; }
            if (body["result"]["_type"] == null || (string)body["result"]["_type"] != "EVENT") { return; }
            if (body["result"]["resourceId"] == null) { return; }
            switch ((string)body["result"]["resourceId"])
            {
                case "ScenesService.sceneSwitched":
                    if (SceneChanged != null)
                    {
                        SceneChanged(this, new SLOBSSceneEvent { Type = ESLOBSEventType.SceneSwitched, ResourceId = (string)body["result"]["resourceId"], Scene = new SLOBSScene ((JObject)body["result"]["data"] )});
                    }
                    break;
                case "ScenesService.sceneAdded":
                    if (SceneChanged != null)
                    {
                        SceneChanged(this, new SLOBSSceneEvent { Type = ESLOBSEventType.SceneAdded, ResourceId = (string)body["result"]["resourceId"], Scene = new SLOBSScene((JObject)body["result"]["data"]) });
                    }
                    break;
                case "ScenesService.sceneRemoved":
                    if (SceneChanged != null)
                    {
                        SceneChanged(this, new SLOBSSceneEvent { Type = ESLOBSEventType.SceneRemoved, ResourceId = (string)body["result"]["resourceId"], Scene = new SLOBSScene((JObject)body["result"]["data"]) });
                    }
                    break;
                case "ScenesService.itemAdded":
                    if (SceneItemChanged != null)
                    {
                        SceneItemChanged(this, new SLOBSSceneItemEvent { Type = ESLOBSEventType.SceneItemAdded, ResourceId = (string)body["result"]["resourceId"], SceneItem = new SLOBSSceneItem((JObject)body["result"]["data"]) });
                    }
                    break;
                case "ScenesService.itemRemoved":
                    if (SceneItemChanged != null)
                    {
                        SceneItemChanged(this, new SLOBSSceneItemEvent { Type = ESLOBSEventType.SceneItemRemoved, ResourceId = (string)body["result"]["resourceId"], SceneItem = new SLOBSSceneItem((JObject)body["result"]["data"]) });
                    }
                    break;
                case "ScenesService.itemUpdated":
                    if (SceneItemChanged != null)
                    {
                        SceneItemChanged(this, new SLOBSSceneItemEvent { Type = ESLOBSEventType.SceneItemUpdated, ResourceId = (string)body["result"]["resourceId"], SceneItem = new SLOBSSceneItem((JObject)body["result"]["data"]) });
                    }
                    break;
                case "StreamingService.streamingStatusChange":
                    if (StreamingStatusChanged != null)
                    {
                        var str = (string)body["result"]["data"];
                        var estr = str.ToUpper().Substring(0, 1) + str.ToLower().Substring(1);
                        try
                        {
                            ESLOBSStreamingState state = (ESLOBSStreamingState)Enum.Parse(typeof(ESLOBSStreamingState), estr);
                            StreamingStatusChanged(this, state);
                        }catch(Exception e)
                        {}
                    }
                    break;
                case "SourcesService.sourceAdded":
                    if (SourceChanged != null)
                    {
                        SourceChanged(this, new SLOBSSourceEvent { Type = ESLOBSEventType.SourceAdded, ResourceId = (string)body["result"]["resourceId"], Source = new SLOBSSource((JObject)body["result"]["data"]) });
                    }
                    break;
                case "SourcesService.sourceRemoved":
                    if (SourceChanged != null)
                    {
                        SourceChanged(this, new SLOBSSourceEvent { Type = ESLOBSEventType.SourceRemoved, ResourceId = (string)body["result"]["resourceId"], Source = new SLOBSSource((JObject)body["result"]["data"]) });
                    }
                    break;
                case "SourcesService.sourceUpdated":
                    if (SourceChanged != null)
                    {
                        SourceChanged(this, new SLOBSSourceEvent { Type = ESLOBSEventType.SourceUpdated, ResourceId = (string)body["result"]["resourceId"], Source = new SLOBSSource((JObject)body["result"]["data"]) });
                    }
                    break;
            }
        }

        private void _connectionStatus(ESLOBSConnectionState s, string msg = null)
        {
            if (ConnectionStatus != null)
                ConnectionStatus(this, new SSLOBSConnectionEvent{ state = s, message = msg });
        }
    }
}
