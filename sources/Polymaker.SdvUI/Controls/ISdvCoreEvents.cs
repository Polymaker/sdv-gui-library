using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    internal interface ISdvCoreEvents
    {
        void OnMouseDown(MouseEventArgs e);
        void OnMouseUp(MouseEventArgs e);
        void OnMouseMove(MouseEventArgs e);
        bool HandleScrollWheel(MouseEventArgs data);
        bool ForwardScrollWheel(MouseEventArgs data);
    }
}
