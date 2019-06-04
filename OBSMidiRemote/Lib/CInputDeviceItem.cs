using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBSMidiRemote.Lib
{
    public class InputDeviceItem
    {
        public string Text { get; set; }
        public object Value { get; set; }
        public InputDeviceType Type { get; set; }
        public int Index { get; set; }

        public InputDeviceItem(String txt = null, int index = -1)
        {
            if (txt != null) { Text = txt; }
            Index = index;
            Type = InputDeviceType.UNKNOWN;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
