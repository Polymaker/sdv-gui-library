using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public interface IScrollableContainer
    {
        Point MinScrollSize { get; set; }
        Point ScrollOffset { get; set; }
        
    }
}
