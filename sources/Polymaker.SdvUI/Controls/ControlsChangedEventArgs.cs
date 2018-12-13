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
            Controls = new SdvControl[] { control };
            ChangeType = changeType;
        }

        public ControlsChangedEventArgs(IEnumerable<SdvControl> controls, Action changeType)
        {
            Controls = controls;
            ChangeType = changeType;
        }

        public IEnumerable<SdvControl> Controls { get; }
        
        public Action ChangeType { get; }
    }
}
