using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBSMidiRemote.Lib
{
    public delegate void StatusChanged(CDeviceObsGw sender, EMidiEvent eventType);
    public delegate void IOBSDeviceStatus(IOBSDevice sender, EMidiEvent eventType);
    public delegate void IOBSDeviceData(IOBSDevice sender, SMidiAction data);
    public delegate void OnMidiOutput(CMidiObserver sender, SMidiAction data);

    public struct SMidiOutputStatus
    {
        int address; // data1
        EMidiOBSItemType item;
        EMidiOBSOutputType outputType;
    }

    public struct SMidiAction
    {
        public int Cmd;
        public int Channel;
        public int Data1;
        public int Data2;
    }


    public struct SMidiOutput
    {
        public EMidiOBSOutputType Type;
        public SMidiAction Action;
    }

    public struct SMidiInput
    {
        public EMidiOBSInputType Type;
        public SMidiAction Action;
    }

    public struct SMidiOBSAction
    {
        public EMidiOBSItemType Type;
        public int ObsMode;
        public bool IsModifier;
        public int Modifier;
        public int Index;
        public List<SMidiInput> InActions;
        public List<SMidiOutput> OutActions;
    }

}
