using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public interface ISdvContainer : ISdvUIComponent
    {
        //Point ScrollOffset { get; set; }
        SdvControlCollection Controls { get; }
        //Rectangle GetClientRectangle();
        IEnumerable<SdvControl> GetVisibleControls();
    }
}
