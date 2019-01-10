using Microsoft.Xna.Framework;
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
        bool Enabled { get; set; }
        bool Visible { get; set; }
        Rectangle Bounds { get; }
        Rectangle ClientRectangle { get; }
        Rectangle DisplayRectangle { get; }
        Rectangle ScreenBounds { get; }
        Rectangle GetScreenBounds();
        Point PointToDisplay(Point localPoint);
        Point PointToLocal(Point displayPoint);
        ISdvContainer Parent { get; }
    }
}
