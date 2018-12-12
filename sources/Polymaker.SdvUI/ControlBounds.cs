using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Polymaker.SdvUI
{
    [Flags]
    public enum ControlBounds
    {
        X = 0x1,
        Y = 0x2,
        Width = 0x4,
        Height = 0x8,
        Location = 0x3,
        Size = 0xC,
        All = 0xF,
        None = 0x0
    }
}
