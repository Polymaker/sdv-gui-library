using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls.Events
{
    public class ValueChangingEventArgs<T> : EventArgs
    {
        public T OldValue { get; }
        public T NewValue { get; }
        public bool Cancel { get; set; }

        public ValueChangingEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
