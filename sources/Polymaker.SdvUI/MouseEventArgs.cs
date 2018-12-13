using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI
{
    public class MouseEventArgs : EventArgs
    {
        public MouseButtons Button { get; }
        public Point Location { get;  }
        public Point DisplayLocation { get; }

        public int X => Location.X;
        public int Y => Location.Y;

        public MouseEventArgs(Point location, MouseButtons button)
        {
            Location = location;
            Button = button;
        }

        public MouseEventArgs(Point location, Point displayLocation, MouseButtons button)
        {
            Location = location;
            DisplayLocation = displayLocation;
            Button = button;
        }
    }
}
