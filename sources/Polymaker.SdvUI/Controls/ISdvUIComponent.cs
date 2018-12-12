﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public interface ISdvUIComponent
    {
        int X { get; set; }
        int Y { get; set; }
        Point Position { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        Point Size { get; set; }
        Rectangle Bounds { get; }
        Rectangle GetDisplayRectangle();
        ISdvContainer Parent { get; }
    }
}
