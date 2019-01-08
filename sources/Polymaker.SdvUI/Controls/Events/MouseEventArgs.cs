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
        public MouseButtons Buttons { get; }
        public bool LeftButton => Buttons.HasFlag(MouseButtons.Left);
        public bool RightButton => Buttons.HasFlag(MouseButtons.Right);
        public bool MiddleButton => Buttons.HasFlag(MouseButtons.Middle);
        public Point Location { get;  private set; }
        public Point DisplayLocation { get; private set; }

        public int X => Location.X;
        public int Y => Location.Y;
        public int Delta { get; }

        public MouseEventArgs(Point location, MouseButtons button)
        {
            Location = location;
            DisplayLocation = location;
            Buttons = button;
        }

        public MouseEventArgs(Microsoft.Xna.Framework.Input.MouseState state)
        {
            Location = new Point(state.X, state.Y);
            DisplayLocation = Location;
            Buttons =  MouseButtons.None;
            if (state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Buttons |= MouseButtons.Left;
            if (state.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Buttons |= MouseButtons.Right;
            if (state.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Buttons |= MouseButtons.Middle;

            Delta = state.ScrollWheelValue;
        }

        public MouseEventArgs(Microsoft.Xna.Framework.Input.MouseState state, int delta)
        {
            Location = new Point(state.X, state.Y);
            DisplayLocation = Location;
            Buttons = MouseButtons.None;
            if (state.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Buttons |= MouseButtons.Left;
            if (state.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Buttons |= MouseButtons.Right;
            if (state.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Buttons |= MouseButtons.Middle;

            Delta = delta;
        }

        public MouseEventArgs(Point location, Point displayLocation, MouseButtons button)
        {
            Location = location;
            DisplayLocation = displayLocation;
            Buttons = button;
        }

        public MouseEventArgs(Point location, Point displayLocation, MouseButtons button, int delta)
        {
            Location = location;
            DisplayLocation = displayLocation;
            Buttons = button;
            Delta = delta;
        }

        public MouseEventArgs ToLocal(Controls.SdvControl control)
        {
            return new MouseEventArgs(control.PointToLocal(DisplayLocation), DisplayLocation, Buttons, Delta);
        }
    }
}
