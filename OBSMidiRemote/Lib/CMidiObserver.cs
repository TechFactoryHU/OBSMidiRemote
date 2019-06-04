using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Text;
using System.Threading.Tasks;
using OBSMidiRemote.Lib.PureMidi.Data;
using OBSMidiRemote.Lib.PureMidi.Definitions;
using OBSMidiRemote.Lib.OBSWebsocketdotnet;

namespace OBSMidiRemote.Lib
{
    public class CMidiObserver
    {
        public List<SMidiOBSAction> midiActions;
        public event OnMidiOutput OnMidiOutput;
        private Dictionary<string, SMidiAction> midiQueue;
        private Dictionary<string, SMidiAction> midiOutputStatus;

        private CDeviceObsGw obs;
        private int active_modifier = 0;
        private int packetCount = 0;
        private int packetInterval = 0;
        public int serialBaudRate = 115200;

        private System.Timers.Timer midiQueueTimer;

        public CMidiObserver(CDeviceObsGw refobs) {
            obs = refobs;
            midiActions = new List<SMidiOBSAction>();
            midiQueue = new Dictionary<string, SMidiAction>();
            midiOutputStatus = new Dictionary<string, SMidiAction>();

            midiQueueTimer = new System.Timers.Timer(1);
            midiQueueTimer.Elapsed += OnMidiQueueTimer;
            midiQueueTimer.AutoReset = true;
            midiQueueTimer.Enabled = false;
        }

        public bool loadSchema(string filename)
        {
            CMidiXMLSchema parser = new CMidiXMLSchema();
            if (parser.loadSchema(filename, ref midiActions, ref packetCount, ref packetInterval, ref serialBaudRate))
            {

                if (packetInterval > 0)
                {
                    midiQueueTimer.Enabled = false;
                    midiQueueTimer.Interval = packetInterval;
                }
                else { midiQueueTimer.Enabled = false; }

                return true;
            }
            return false;
        }

        public void ParseInputData(SMidiAction action)
        {
            for (int m = 0; m < midiActions.Count(); m++)
            {
                if (obs.Mode == midiActions[m].ObsMode || midiActions[m].ObsMode == 0)
                {
                    if (midiActions[m].Type == EMidiOBSItemType.Modifier || 
                        (midiActions[m].Type != EMidiOBSItemType.Modifier && midiActions[m].Modifier == active_modifier) ||
                        (midiActions[m].Type != EMidiOBSItemType.Modifier && midiActions[m].Modifier == -1)
                    )
                    {
                        for (int i = 0; i < midiActions[m].InActions.Count(); i++)
                        {
                            if (action.Cmd == midiActions[m].InActions[i].Action.Cmd &&
                                action.Data1 == midiActions[m].InActions[i].Action.Data1
                                )
                            {
                                //Modifier button pressed
                                if (midiActions[m].Type == EMidiOBSItemType.Modifier)
                                {
                                    if (action.Data2 == midiActions[m].InActions[i].Action.Data2)
                                    {
                                        if (midiActions[m].InActions[i].Type == EMidiOBSInputType.On)
                                        {
                                            active_modifier = midiActions[m].Modifier;
                                        }
                                        else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Off)
                                        {
                                            active_modifier = 0;
                                        }
                                        else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Toggle)
                                        {
                                            if (active_modifier != midiActions[m].Modifier)
                                            {
                                                active_modifier = midiActions[m].Modifier;
                                            }
                                            else
                                            {
                                                active_modifier = 0;
                                            }
                                        }
                                        RenderSurface();
                                    }
                                }

                                //Input modifier match or any modifier
                                else if (midiActions[m].Modifier == active_modifier || midiActions[m].Modifier == -1)
                                {
                                    //Data2 match or any Data2 value
                                    if (action.Data2 == midiActions[m].InActions[i].Action.Data2 || midiActions[m].InActions[i].Action.Data2 == -1)
                                    {
                                        //
                                        // Scene 
                                        //
                                        if (midiActions[m].Type == EMidiOBSItemType.Scene || midiActions[m].Type == EMidiOBSItemType.PScene)
                                        {
                                            
                                            if (midiActions[m].InActions[i].Type == EMidiOBSInputType.On)
                                            {
                                                if (midiActions[m].Index < obs.Scenes.Count())
                                                {
                                                    Console.WriteLine(midiActions[m].Type + "=>" + obs.Scenes[midiActions[m].Index].Name);

                                                    if (midiActions[m].Type == EMidiOBSItemType.PScene)
                                                    {
                                                        obs.SetPreviewScene(obs.Scenes[midiActions[m].Index].Name);
                                                    }
                                                    else
                                                    {
                                                        obs.SetCurrentScene(obs.Scenes[midiActions[m].Index].Name);
                                                    }
                                                }
                                            }
                                        }

                                        //
                                        // Scene Item
                                        //

                                        else if (midiActions[m].Type == EMidiOBSItemType.SceneItem || midiActions[m].Type == EMidiOBSItemType.PSceneItem)
                                        {
                                            int sscene_id = midiActions[m].Type == EMidiOBSItemType.PSceneItem && obs.Mode == 2 ? obs.ActivePScene : obs.ActiveScene;
                                            if (midiActions[m].Type == EMidiOBSItemType.PSceneItem && obs.Mode == 2)
                                            {
                                                Console.WriteLine("PScene:" + sscene_id + "@" + midiActions[m].Index);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Scene:" + sscene_id + "@" + midiActions[m].Index);
                                            }


                                            if (sscene_id < obs.Scenes.Count())
                                            {
                                                if (midiActions[m].InActions[i].Type == EMidiOBSInputType.On)
                                                {
                                                    obs.SetSceneItemVisibility(sscene_id, midiActions[m].Index, true);
                                                }
                                                else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Off)
                                                {
                                                    obs.SetSceneItemVisibility(sscene_id, midiActions[m].Index, false);
                                                }
                                                else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Toggle)
                                                {
                                                    obs.ToggleSceneItemVisibility(sscene_id, midiActions[m].Index);
                                                }
                                            }
                                        }

                                        //
                                        // Audio Item (Mute buttons)
                                        //
                                        else if (midiActions[m].Type == EMidiOBSItemType.AudioItem)
                                        {
                                            if (midiActions[m].InActions[i].Type == EMidiOBSInputType.On)
                                            {
                                                obs.SetSourceAudioMute(midiActions[m].Index, true);
                                            }
                                            else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Off)
                                            {
                                                obs.SetSourceAudioMute(midiActions[m].Index, false);
                                            }
                                            else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Toggle)
                                            {
                                                obs.ToggleSourceAudioMute(midiActions[m].Index);
                                            }
                                        }

                                        //
                                        // Reload all data from OBS
                                        //
                                        else if (midiActions[m].Type == EMidiOBSItemType.ReloadOBSData)
                                        {
                                            if (midiActions[m].InActions[i].Type == EMidiOBSInputType.On)
                                            {
                                                obs.Refresh();
                                            }
                                        }

                                        //
                                        // Start/Stop streaming
                                        //
                                        else if (midiActions[m].Type == EMidiOBSItemType.Stream)
                                        {
                                            if (midiActions[m].InActions[i].Type == EMidiOBSInputType.On)
                                            {
                                                obs.StartStreaming();
                                            }
                                            else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Off)
                                            {
                                                obs.StopStreaming();
                                            }
                                            else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Toggle)
                                            {
                                                if (obs.OutputStatus.IsStreaming) { obs.StopStreaming(); }
                                                else { obs.StartStreaming(); }
                                            }
                                        }

                                        //
                                        // Start/Stop recording
                                        //
                                        else if (midiActions[m].Type == EMidiOBSItemType.Record)
                                        {
                                            if (midiActions[m].InActions[i].Type == EMidiOBSInputType.On)
                                            {
                                                obs.StartRecording();
                                            }
                                            else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Off)
                                            {
                                                obs.StopRecording();
                                            }
                                            else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Toggle)
                                            {
                                                if (obs.OutputStatus.IsRecording) { obs.StopRecording(); }
                                                else { obs.StartRecording(); }
                                            }
                                        }

                                        //
                                        // Switch mode
                                        //
                                        else if (midiActions[m].Type == EMidiOBSItemType.Mode)
                                        {
                                            if (midiActions[m].InActions[i].Type == EMidiOBSInputType.On)
                                            {
                                                if (obs.Mode != 2)
                                                {
                                                    obs.SetStudioMode(true);
                                                }
                                            }
                                            else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Off)
                                            {
                                                if (obs.Mode == 2)
                                                {
                                                    obs.SetStudioMode(false);
                                                }
                                            }
                                            else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Toggle)
                                            {
                                                obs.SetStudioMode(obs.Mode != 2 ? true : false);
                                            }
                                        }

                                        //
                                        // Transition item
                                        //
                                        else if (midiActions[m].Type == EMidiOBSItemType.Transition)
                                        {
                                            if (midiActions[m].Index < obs.Transitions.Count())
                                            {
                                                if (midiActions[m].InActions[i].Type == EMidiOBSInputType.On || midiActions[m].InActions[i].Type == EMidiOBSInputType.Toggle)
                                                {
                                                    obs.SetTransition(midiActions[m].Index);
                                                }

                                            }
                                        }

                                    //Data2 match not required
                                    }
                                    else
                                    {
                                        //
                                        // Audio Volume (for sliders)
                                        //
                                        if (midiActions[m].Type == EMidiOBSItemType.AudioVolume)
                                        {
                                            if (midiActions[m].InActions[i].Type == EMidiOBSInputType.On)
                                            {
                                                obs.SetSourceAudioVolume(midiActions[m].Index, 1);
                                            }
                                            else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Off)
                                            {
                                                obs.SetSourceAudioVolume(midiActions[m].Index, 0);
                                            }
                                            else if (midiActions[m].InActions[i].Type == EMidiOBSInputType.Value)
                                            {
                                                float mappedValue = Map(action.Data2, 0, 127, 0, 1);
                                                obs.SetSourceAudioVolume(midiActions[m].Index, mappedValue);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }//obsmode
            }//for
            
        }

        private float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        public void ResetSurface()
        {
           // midiOutputStatus.Clear();
            Display(EMidiOBSItemType.ConnectionStatus, EMidiOBSOutputType.Off, -1, -1, true);
            Display(EMidiOBSItemType.Stream, EMidiOBSOutputType.Off, -1, -1, true);
            Display(EMidiOBSItemType.Record, EMidiOBSOutputType.Off, -1, -1, true);
            Display(EMidiOBSItemType.Scene, EMidiOBSOutputType.Off, -1, -1, true);
            Display(EMidiOBSItemType.SceneItem, EMidiOBSOutputType.Off, -1, -1, true);
            Display(EMidiOBSItemType.AudioItem, EMidiOBSOutputType.Off, -1, -1, true);
            Display(EMidiOBSItemType.Transition, EMidiOBSOutputType.Off, -1, -1, true);
            Display(EMidiOBSItemType.PScene, EMidiOBSOutputType.Off, -1, -1, true);
            Display(EMidiOBSItemType.PSceneItem, EMidiOBSOutputType.Off, -1, -1, true);
            Display(EMidiOBSItemType.Mode, EMidiOBSOutputType.Off, -1, -1, true);
            Display(EMidiOBSItemType.ReloadOBSData, EMidiOBSOutputType.Off, -1, -1, true);
            Display(EMidiOBSItemType.Modifier, EMidiOBSOutputType.Off, -1, -1, true);
        }

        public bool checkLastOutputStatus(SMidiAction o)
        {
            string targetId = o.Data1.ToString("X2"); // + "_" + o.Channel.ToString("X2")
            if (midiOutputStatus.ContainsKey(targetId))
            {
                if (midiOutputStatus[targetId].Cmd == o.Cmd 
                    && midiOutputStatus[targetId].Data2 == o.Data2
                    && midiOutputStatus[targetId].Channel == o.Channel
                )
                {
                    return false;
                }

                midiOutputStatus[targetId] = o;
            }
            else { midiOutputStatus.Add(targetId, o); }
            return true;
        }

        public void Display(EMidiOBSItemType item, EMidiOBSOutputType t, int ix = 0, int obsmode = 0, bool forced = false)
        {
            if (obsmode == 0) { obsmode = obs.Mode; }
            else if (obsmode == -1) { obsmode = 0; }
           
            //Console.WriteLine("Display;  obsmode=" + obsmode + ", item=" + item + ", type=" + t + ", index=" + ix + ", forced=" + forced);
            
            for (int m = 0; m < midiActions.Count(); m++)
            {
                if (obsmode == 0 || obsmode == midiActions[m].ObsMode || midiActions[m].ObsMode == 0)
                {
                    if ( (midiActions[m].Type == item && (item == EMidiOBSItemType.Modifier || active_modifier == midiActions[m].Modifier || midiActions[m].Modifier == -1 ) && (midiActions[m].Index == ix || ix == -1)))
                    {
                 
                        //Console.WriteLine("--> type " + midiActions[m].Type + ", active_modifier=" + active_modifier + ", modifier=" + midiActions[m].Modifier + ", index=" + midiActions[m].Index + " [OK]");
                        foreach (SMidiOutput o in midiActions[m].OutActions)
                        {
                            if (o.Type == t)
                            {
                                
                                //Console.WriteLine("----> Out " + o.Type + " [OK]");
                                //track last status
                                if (checkLastOutputStatus(o.Action)) {
                                    if (forced) { _sendMidiCmd(o.Action); } else { SendMidiCommand(o.Action); }
                                }
                            }
                        }
                    }
                   /* else if (midiActions[m].Type == SMidiOBSItemType.Modifier)
                    {
                        //Console.WriteLine("--> ismodifier " + midiActions[m].Type + " [OK]");
                        foreach (SMidiOutput o in midiActions[m].OutActions)
                        {
                            if (o.Type == t && (midiActions[m].Modifier == ix || ix == -1))
                            {
                                //track last status
                                if (checkLastOutputStatus(o.Action))
                                {
                                    if (forced) { _sendMidiCmd(o.Action); } else { SendMidiCommand(o.Action); }
                                }
                            }
                        }
                    }*/
                   /* else if (active_modifier != midiActions[m].Modifier && midiActions[m].Modifier != -1)
                    {
                        //Console.WriteLine("--> isNotCorrectModifier active_modifier="+active_modifier+", modifier=" + midiActions[m].Modifier + " [OK]");
                        foreach (SMidiOutput o in midiActions[m].OutActions)
                        {
                            if (o.Type == SMidiOBSOutputType.Off)
                            {
                                //track last status
                                if (checkLastOutputStatus(o.Action))
                                {
                                    if (forced) { _sendMidiCmd(o.Action); } else { SendMidiCommand(o.Action); }
                                }    
                            }
                        }
                    }*/
                }
            }
        }

        public void RenderSurface()
        {
            //reset scenes
            for (int m = 0; m < midiActions.Count(); m++)
            {
                if (midiActions[m].Type == EMidiOBSItemType.Modifier)
                {
                    if (midiActions[m].Modifier == active_modifier)
                    {
                        Display(EMidiOBSItemType.Modifier, EMidiOBSOutputType.Active, midiActions[m].Index);
                    }
                    else
                    {
                        Display(EMidiOBSItemType.Modifier, EMidiOBSOutputType.On, midiActions[m].Index);
                    }
                }
                else
                {
                    if (midiActions[m].Modifier != -1)
                    {
                        Display(midiActions[m].Type, EMidiOBSOutputType.Off, midiActions[m].Index);
                    }

                }
            }

            //scene part
           // Display(SMidiOBSItemType.Scene, SMidiOBSOutputType.Off, -1, -1);
           // Display(SMidiOBSItemType.SceneItem, SMidiOBSOutputType.Off, -1, -1);
          //  Display(SMidiOBSItemType.AudioItem, SMidiOBSOutputType.Off, -1, -1);
          //  Display(SMidiOBSItemType.AudioVolume, SMidiOBSOutputType.Off, -1, -1);
          //  Display(SMidiOBSItemType.ReloadOBSData, SMidiOBSOutputType.Off, -1, -1);
          //  Display(SMidiOBSItemType.Stream, SMidiOBSOutputType.Off, -1, -1);
          //  Display(SMidiOBSItemType.Record, SMidiOBSOutputType.Off, -1, -1);
           // Display(SMidiOBSItemType.Transition, SMidiOBSOutputType.Off, -1, -1);
            //Display(SMidiOBSItemType.Modifier, SMidiOBSOutputType.On, -1, -1);
           // Display(SMidiOBSItemType.Mode, SMidiOBSOutputType.On, -1, -1);

            if (obs.Connected())
            {
                //ObsMode
                Display(EMidiOBSItemType.Mode, obs.Mode == 2 ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, -1);

                //Websocket status
                Display(EMidiOBSItemType.ConnectionStatus, EMidiOBSOutputType.On, -1);

                //scenes & sources
                for (int i = 0; i < obs.Scenes.Count(); i++)
                {
                    if (obs.ActiveSceneName == obs.Scenes[i].Name)
                    {
                        Display(EMidiOBSItemType.Scene, EMidiOBSOutputType.Active, i, obs.Mode);
                        for (int s = 0; s < obs.Scenes[i].Items.Count(); s++)
                        {
                            Display(EMidiOBSItemType.SceneItem, obs.Scenes[i].Items[s].Visible ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, s);
                        }
                    }
                    else
                    {
                        Display(EMidiOBSItemType.Scene, EMidiOBSOutputType.On, i);
                    }

                    if (obs.Mode == 2)
                    {
                        if (obs.ActivePSceneName == obs.Scenes[i].Name)
                        {
                            Display(EMidiOBSItemType.PScene, EMidiOBSOutputType.Active, i);

                            for (int s = 0; s < obs.Scenes[i].Items.Count(); s++)
                            {
                                Display(EMidiOBSItemType.PSceneItem, obs.Scenes[i].Items[s].Visible ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, s);
                            }
                        }
                        else
                        {
                            Display(EMidiOBSItemType.PScene, EMidiOBSOutputType.On, i);
                        }
                    }
                }

                //audio 
                int _audiosources = 0;
                foreach (var item in obs.SourceItems)
                {
                    if (obs.SourceHasAudio(item.TypeId))
                    {
                        if (item.Muted || item.Volume == 0)
                        {
                            Display(EMidiOBSItemType.AudioItem, EMidiOBSOutputType.Muted, _audiosources);
                        }
                        else
                        {
                            Display(EMidiOBSItemType.AudioItem, EMidiOBSOutputType.On, _audiosources);
                            if (item.Volume > 0)
                            {
                                Display(EMidiOBSItemType.AudioVolume, EMidiOBSOutputType.On, _audiosources);
                            }
                            else
                            {
                                Display(EMidiOBSItemType.AudioVolume, EMidiOBSOutputType.Muted, _audiosources);
                            }
                        }
                        _audiosources++;
                    }
                }

                //transitions
                int tr_i = 0;
                foreach (var tr_name in obs.Transitions)
                {
                    if (tr_name == obs.CurrentTransition.Name)
                    {
                        Display(EMidiOBSItemType.Transition, EMidiOBSOutputType.Active, tr_i);
                    }
                    else
                    {
                        Display(EMidiOBSItemType.Transition, EMidiOBSOutputType.On, tr_i);
                    }
                    tr_i++;
                }

                Display(EMidiOBSItemType.ReloadOBSData, EMidiOBSOutputType.On, -1);
                Display(EMidiOBSItemType.Stream, obs.OutputStatus.IsStreaming ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, -1);
                Display(EMidiOBSItemType.Record, obs.OutputStatus.IsRecording ? EMidiOBSOutputType.Active : EMidiOBSOutputType.On, -1);
            }
            else
            {
                Display(EMidiOBSItemType.ConnectionStatus, EMidiOBSOutputType.Off, -1);
            }
            FlushQueue();
        }

        public void SendMidiCommand(SMidiAction action)
        {
            string targetId = action.Data1.ToString("X2") + "_" + action.Channel.ToString("X2");
            if (midiQueue.ContainsKey(targetId))
            {
                midiQueue[targetId] = action;
            }
            else
            {
                midiQueue.Add(targetId, action);
            }

            FlushQueue();
        }

        public void FlushQueue()
        {
            if (midiQueue.Count() > 0)
            {
                if (midiQueueTimer.Enabled && packetCount > 0) { midiQueueTimer.Start(); }
                int i = 0;
                foreach (var item in midiQueue.ToList())
                {
                    if (item.Key != null)
                    {
                        _sendMidiCmd(item.Value);
                        midiQueue.Remove(item.Key);
                        i++;

                        if (midiQueueTimer.Enabled && packetCount > 0)
                        {
                            if (i + 1 >= packetCount) { break; }
                        }
                    }
                }
            }
            else
            {
                if (midiQueueTimer.Enabled && packetCount > 0) { midiQueueTimer.Stop(); }
                midiQueue.Clear();
            }
        }

        private void OnMidiQueueTimer(Object source, ElapsedEventArgs e)
        {
            FlushQueue();
        }

        private void _sendMidiCmd(SMidiAction action)
        {

            if (OnMidiOutput != null) { OnMidiOutput(this, action); }
        }
    }
}
