using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI
{
    public class SdvMouseEventArgs : EventArgs
    {
        public Vector2 Position => new Vector2(X, Y);
        public int X => State.X;
        public int Y => State.Y;

        public MouseState State { get; private set; }

        public SdvMouseEventArgs(MouseState state)
        {
            State = state;
        }
    }
}
