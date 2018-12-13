using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    internal interface ISdvContainerCore
    {
        IEnumerable<SdvControl> AllControls { get; }
    }
}
