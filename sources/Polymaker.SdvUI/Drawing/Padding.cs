using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI
{
    public struct Padding
    {
        public static readonly Padding Empty = new Padding(0);

        private bool _all;

        private int _top;

        private int _left;

        private int _right;

        private int _bottom;
        
        public int All
        {
            get
            {
                if (!_all)
                    return -1;
                return _top;
            }
            set
            {
                if (_all && _top == value)
                    return;
                _all = true;
                _top = _left = _right = _bottom = value;
            }
        }

        public int Bottom
        {
            get
            {
                if (_all)
                {
                    return _top;
                }
                return _bottom;
            }
            set
            {
                if (!_all && _bottom == value)
                    return;
                _all = false;
                _bottom = value;
            }
        }

        public int Left
        {
            get
            {
                if (_all)
                    return _top;
                return _left;
            }
            set
            {
                if (!_all && _left == value)
                    return;
                _all = false;
                _left = value;
            }
        }

        public int Right
        {
            get
            {
                if (_all)
                    return _top;
                return _right;
            }
            set
            {
                if (!_all && _right == value)
                    return;
                _all = false;
                _right = value;
            }
        }

        public int Top
        {
            get
            {
                return _top;
            }
            set
            {
                if (!_all && _top == value)
                    return;
                _all = false;
                _top = value;
            }
        }

        public int Horizontal
        {
            get
            {
                return Left + Right;
            }
        }

        public int Vertical
        {
            get
            {
                return Top + Bottom;
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2(Horizontal, Vertical);
            }
        }


        public Padding(int all)
        {
            _all = true;
            _top = (_left = (_right = (_bottom = all)));
        }


        public Padding(int left, int top, int right, int bottom)
        {
            _top = top;
            _left = left;
            _right = right;
            _bottom = bottom;
            _all = (_top == _left && _top == _right && _top == _bottom);
        }

        public override bool Equals(object other)
        {
            if (other is Padding)
            {
                return (Padding)other == this;
            }
            return false;
        }

        public static bool operator ==(Padding p1, Padding p2)
        {
            return p1.Left == p2.Left && p1.Top == p2.Top && p1.Right == p2.Right && p1.Bottom == p2.Bottom;
        }

        public static bool operator !=(Padding p1, Padding p2)
        {
            return !(p1 == p2);
        }

        public override int GetHashCode()
        {
            var hashCode = -773114317;
            hashCode = hashCode * -1521134295 + Bottom.GetHashCode();
            hashCode = hashCode * -1521134295 + Left.GetHashCode();
            hashCode = hashCode * -1521134295 + Right.GetHashCode();
            hashCode = hashCode * -1521134295 + Top.GetHashCode();
            return hashCode;
        }
    }
}
