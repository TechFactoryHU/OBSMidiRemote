using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBSMidiRemote.Lib
{
    public interface IOBSDevice
    {

        event IOBSDeviceStatus OnStatusChanged;
        event IOBSDeviceData OnData;

        int BaudRate
        {
            get;
            set;
        }

        InputDeviceItem Input {
            get;
            set;    
        }

        InputDeviceItem Output
        {
            get;
            set;
        }

        bool Connect();

        void Disconnect();

        bool IsReady();

        void Send(SMidiAction data);


    }
}
