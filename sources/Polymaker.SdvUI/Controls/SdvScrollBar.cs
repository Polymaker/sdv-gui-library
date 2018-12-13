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

        public int LargeChange { get; set; }

        public int SmallChange { get; set; }

        public bool WheelScrollLarge { get; set; }

        public event EventHandler Scroll;

        public SdvScrollBar(Orientation orientation)
        {
            Orientation = orientation;
            _MaxValue = 1;
            SmallChange = 8;
            LargeChange = 32;
            if (Orientation == Orientation.Horizontal)
                Height = SCROLLBAR_SIZE;
            else
                Width = SCROLLBAR_SIZE;
        }

        public SdvScrollBar(Orientation orientation, bool fromContainer)
        {
            Orientation = orientation;
            _MaxValue = 1;
            SmallChange = 8;
            LargeChange = 32;
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

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                if (UpArrowBounds.Contains(e.DisplayLocation))
                {
                    if (Value > 0)
                        Value -= LargeChange;
                }
                else if (DownArrowBounds.Contains(e.DisplayLocation))
                {
                    if (Value < MaxValue)
                        Value += LargeChange;
                }
                else if (ScrollbarTrackBounds.Contains(e.DisplayLocation))
                {
                    var clickedPos = GetScrollValueAtPosition(e.DisplayLocation);
                    Value = clickedPos;
                }
            }
        }

        private bool MouseDragging = false;
        private Vector2 DragStart;
        private int DragStartValue;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                var displayPt = PointToDisplay(e.Location);
                if (ScrollbarButtonBounds.Contains(displayPt))
                {
                    MouseDragging = true;
                    DragStartValue = Value;
                    var worldPos = PointToDisplay(e.Location);
                    DragStart = new Vector2(worldPos.X, worldPos.Y);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if(e.Button == MouseButtons.Left && MouseDragging)
            {
                MouseDragging = false;
            }
        }

        protected override void OnScrollWheel(int delta)
        {
            base.OnScrollWheel(delta);
            
            Value += (WheelScrollLarge ? LargeChange : SmallChange) * Math.Sign(delta) * -1;
        }

        public override bool CaptureMouseWheel(int x, int y)
        {
            return Bounds.Contains(x, y);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (MouseDragging)
            {
                //var curPos = new Vector2(e.DisplayLocation.X, e.DisplayLocation.Y);
                //var dragDelta = curPos - DragStart;
                //var newValueRatio = 0f;

                //if (Orientation == Orientation.Vertical)
                //    newValueRatio = (DragStart.Y - ScrollbarTrackBounds.Y + dragDelta.Y) / (ScrollbarTrackBounds.Height - ScrollbarButtonBounds.Height);
                //else
                //    newValueRatio = (DragStart.X - ScrollbarTrackBounds.X + dragDelta.X) / (ScrollbarTrackBounds.Width - ScrollbarButtonBounds.Height);

                Value = GetScrollValueAtPosition(e.DisplayLocation);
            }
        }

        public int GetScrollValueAtPosition(Point position)
        {
            var positionRatio = 0f;

            if (Orientation == Orientation.Vertical)
                positionRatio = (position.Y - ScrollbarTrackBounds.Y - (ScrollbarButtonBounds.Height / 2f)) / (float)(ScrollbarTrackBounds.Height - ScrollbarButtonBounds.Height);
            else
                positionRatio = (position.X - ScrollbarTrackBounds.X - (ScrollbarButtonBounds.Width / 2f)) / (float)(ScrollbarTrackBounds.Width - ScrollbarButtonBounds.Height);

            var scrollValue = (int)Math.Max(0, Math.Min(MaxValue * positionRatio, MaxValue));

            scrollValue = (int)Math.Round(scrollValue / (float)SmallChange) * SmallChange;

            return scrollValue;
        }

        //protected override void OnUpdate(GameTime delta)
        //{
        //    base.OnUpdate(delta);
        //    if (MouseDragging)
        //    {
        //        var curPos = new Vector2(Cursor.X, Cursor.Y);
        //        var dragDelta = curPos - DragStart;
        //        var newValueRatio = 0f;

        //        if (Orientation == Orientation.Vertical)
        //            newValueRatio = (DragStart.Y - ScrollbarTrackBounds.Y + dragDelta.Y) / (ScrollbarTrackBounds.Height - ScrollbarButtonBounds.Height);
        //        else
        //            newValueRatio = (DragStart.X - ScrollbarTrackBounds.X + dragDelta.X) / (ScrollbarTrackBounds.Width - ScrollbarButtonBounds.Height);

        //        Value = (int)(newValueRatio * MaxValue);
        //    }
        //}
    }
}
