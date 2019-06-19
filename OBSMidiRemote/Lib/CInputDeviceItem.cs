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

    public class StandardListItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public int Index { get; set; }

        public StandardListItem(String txt = null, int index = -1)
        {
            if (txt != null) { Text = txt; }
            Value = index;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
