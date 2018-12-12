using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Polymaker.SdvUI.Controls
{
    public class SdvScrollBar : SdvControl
    {
        private int _MaxValue;
        private int _LargeChange;
        private int _SmallChange;
        private int _Value;
        private bool IsContainerScrollBar;

        private Rectangle UpArrowBounds;
        private Rectangle DownArrowBounds;
        private Rectangle ScrollbarTrackBounds;
        private Rectangle ScrollbarButtonBounds;

        private const int SCROLLBAR_SIZE = 44;
        private const int TRACK_SIZE = 24;

        public Orientation Orientation { get; private set; }

        public int Value
        {
            get => _Value;
            set
            {
                value = Math.Max(0, Math.Min(value, _MaxValue));
                if (value != _Value)
                {
                    _Value = value;
                    OnScroll(EventArgs.Empty);
                }
            }
        }

        public int MaxValue
        {
            get => _MaxValue;
            set
            {
                //value = (int)Math.Max(_SmallChange, Math.Ceiling(value / (float)_SmallChange) * _SmallChange);
                value = Math.Max(value, 1);
                if (value != _MaxValue)
                {
                    _MaxValue = value;
                    if (Value > value)
                        Value = value;
                }
            }
        }

        public event EventHandler Scroll;

        public SdvScrollBar(Orientation orientation)
        {
            Orientation = orientation;
            _MaxValue = 1;
            _SmallChange = 8;
            _LargeChange = 32;
            if (Orientation == Orientation.Horizontal)
                Height = SCROLLBAR_SIZE;
            else
                Width = SCROLLBAR_SIZE;
        }

        public SdvScrollBar(Orientation orientation, bool fromContainer)
        {
            Orientation = orientation;
            _MaxValue = 1;
            _SmallChange = 8;
            _LargeChange = 32;
            IsContainerScrollBar = fromContainer;

            if (Orientation == Orientation.Horizontal)
                Height = SCROLLBAR_SIZE;
            else
                Width = SCROLLBAR_SIZE;
        }

        protected void OnScroll(EventArgs e)
        {
            Scroll?.Invoke(this, e);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, ControlBounds specifiedBounds)
        {
            if (/*specifiedBounds.HasFlag(ControlBounds.Width) && */Orientation == Orientation.Vertical)
            {
                width = SCROLLBAR_SIZE;
            }
            else if (/*specifiedBounds.HasFlag(ControlBounds.Height) && */Orientation == Orientation.Horizontal)
            {
                height = SCROLLBAR_SIZE;
            }

            base.SetBoundsCore(x, y, width, height, specifiedBounds);
        }

        public override Rectangle GetDisplayRectangle()
        {
            if (!IsContainerScrollBar)
                return base.GetDisplayRectangle();

            if (Parent != null)
            {
                var parentBounds = Parent.GetDisplayRectangle();
                return new Rectangle(
                    parentBounds.X + X,
                    parentBounds.Y + Y,
                    Width, Height);
            }
            return Bounds;
        }

        //protected override void OnSizeChanged(EventArgs e)
        //{
        //    base.OnSizeChanged(e);
        //    UpdateScrollbarBounds();
        //}

        //protected override void OnParentChanged(EventArgs e)
        //{
        //    base.OnParentChanged(e);
        //    UpdateScrollbarBounds();
        //}

        private void UpdateScrollbarBounds()
        {
            var bound = GetDisplayRectangle();

            if (Orientation == Orientation.Vertical)
            {
                UpArrowBounds = new Rectangle(bound.X, bound.Y, 44, 48);
                DownArrowBounds = new Rectangle(bound.X, bound.Bottom - 48, 44, 48);
                ScrollbarTrackBounds = new Rectangle(bound.X + 12, UpArrowBounds.Bottom + 4, TRACK_SIZE, Height - (UpArrowBounds.Height + 4) * 2);

                ScrollbarButtonBounds = new Rectangle(ScrollbarTrackBounds.X, ScrollbarTrackBounds.Y, 24, 40);
                ScrollbarButtonBounds.Y += (int)((Value / (float)MaxValue) * (ScrollbarTrackBounds.Height - ScrollbarButtonBounds.Height));
            }
            else
            {
                UpArrowBounds = new Rectangle(bound.X, bound.Y, 48, 44);
                DownArrowBounds = new Rectangle(bound.Right - 48, bound.Y, 48, 44);
                ScrollbarTrackBounds = new Rectangle(UpArrowBounds.Right + 4, bound.Y + 11, Width - (UpArrowBounds.Width + 4) * 2 , TRACK_SIZE);

                ScrollbarButtonBounds = new Rectangle(ScrollbarTrackBounds.X, ScrollbarTrackBounds.Y, 40, 24);
                ScrollbarButtonBounds.X += (int)((Value / (float)MaxValue) * (ScrollbarTrackBounds.Width - ScrollbarButtonBounds.Width));
            }
        }

        protected override void OnDraw(SpriteBatch b)
        {
            base.OnDraw(b);

            UpdateScrollbarBounds();
            //var bound = DisplayBounds;

            b.DrawTextureBox(SdvImages.ScrollBarTrack, ScrollbarTrackBounds, Color.White, 4f, true);

            if (Orientation == Orientation.Vertical)
            {
                b.DrawImage(SdvImages.UpArrow, UpArrowBounds, 4f);
                b.DrawImage(SdvImages.DownArrow, DownArrowBounds, 4f);
                b.DrawImage(SdvImages.VScrollbarButton, ScrollbarButtonBounds, Color.White, 4f, false);
            }
            else
            {
                b.DrawImageRotated(SdvImages.UpArrow, UpArrowBounds, -3.14f / 2f, Color.White, 4f);
                b.DrawImageRotated(SdvImages.DownArrow, DownArrowBounds, -3.14f / 2f, Color.White, 4f);
                b.DrawImage(SdvImages.HScrollbarButton, ScrollbarButtonBounds, Color.White, 4f, false);
            }
        }

        protected internal override void OnLeftClick(Point pos)
        {
            var displayPt = PointToDisplay(pos);
            if (UpArrowBounds.Contains(displayPt))
            {
                if (Value > 0)
                    Value -= _SmallChange;
            }
            else if (DownArrowBounds.Contains(displayPt))
            {
                if (Value < MaxValue)
                    Value += _SmallChange;
            }
        }
    }
}
