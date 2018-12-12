using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class ControlsChangedEventArgs : EventArgs
    {
        public enum Action
        {
            Add,
            Remove,
            Clear
        }

        public ControlsChangedEventArgs(SdvControl control, Action changeType)
        {
            Control = control;
            ChangeType = changeType;
        }

        public SdvControl Control { get; }
        
        public Action ChangeType { get; }
    }
}
