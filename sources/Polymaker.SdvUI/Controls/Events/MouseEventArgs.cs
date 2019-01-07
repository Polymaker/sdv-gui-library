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
        public Point Location { get;  private set; }
        public Point DisplayLocation { get; private set; }

        public int X => Location.X;
        public int Y => Location.Y;
        public int Delta { get; }

        public MouseEventArgs(Point location, MouseButtons button)
        {
            Location = location;
            DisplayLocation = location;
            Button = button;
        }

        public MouseEventArgs(Microsoft.Xna.Framework.Input.MouseState state)
        {
            Location = new Point(state.X, state.Y);
            DisplayLocation = Location;
            Button =  MouseButtons.None;
            if (state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Button |= MouseButtons.Left;
            if (state.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Button |= MouseButtons.Right;
            if (state.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Button |= MouseButtons.Middle;

            Delta = state.ScrollWheelValue;
        }

        public MouseEventArgs(Microsoft.Xna.Framework.Input.MouseState state, int delta)
        {
            Location = new Point(state.X, state.Y);
            DisplayLocation = Location;
            Button = MouseButtons.None;
            if (state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Button |= MouseButtons.Left;
            if (state.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Button |= MouseButtons.Right;
            if (state.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Button |= MouseButtons.Middle;

            Delta = delta;
        }

        public MouseEventArgs(Point location, Point displayLocation, MouseButtons button)
        {
            Location = location;
            DisplayLocation = displayLocation;
            Button = button;
        }

        public MouseEventArgs(Point location, Point displayLocation, MouseButtons button, int delta)
        {
            Location = location;
            DisplayLocation = displayLocation;
            Button = button;
            Delta = delta;
        }

        public MouseEventArgs ToLocal(Controls.SdvControl control)
        {
            return new MouseEventArgs(control.PointToLocal(DisplayLocation), DisplayLocation, Button, Delta);
        }
    }
}
