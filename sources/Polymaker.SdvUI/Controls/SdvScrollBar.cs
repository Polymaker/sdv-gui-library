using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polymaker.SdvUI.Drawing;

namespace Polymaker.SdvUI.Controls
{
    public class SdvScrollBar : SdvControl
    {
        private int _MaxValue;
        private int _Value;
        private int _ScrollButtonSize;
        private float _ScrollButtonRatio;
        private bool IsContainerScrollBar;

        private Rectangle UpArrowBounds;
        private Rectangle DownArrowBounds;
        private Rectangle ScrollbarTrackBounds;
        private Rectangle ScrollbarButtonBounds;

        private const int SCROLLBAR_SIZE = 44;
        public const int SCROLLBUTTON_SIZE = 40;
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
                    OnScrollCore();
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
                    _ScrollButtonRatio = 1f;
                    if (Value > value)
                        Value = value;
                    AdjustMaxValue();
                }
            }
        }

        private int _LargeChange;
        private int _SmallChange;

        public int LargeChange
        {
            get => _LargeChange;
            set
            {
                value = value < SmallChange ? SmallChange : value;
                if (value != _LargeChange)
                {
                    _LargeChange = value;
                    AdjustMaxValue();
                }
            }
        }

        public int SmallChange
        {
            get => _SmallChange;
            set
            {
                value = value < 1 ? 1 : value;
                if (value != _SmallChange)
                {
                    _SmallChange = value;
                    _LargeChange = (int)Math.Ceiling((_LargeChange / (double)value)) * value;
                    AdjustMaxValue();
                }
            }
        }

        public float ScrollButtonRatio
        {
            get => _ScrollButtonRatio;
            set
            {
                if (_ScrollButtonRatio != value)
                {
                    if (value < 1f && value > 0f)
                    {
                        _ScrollButtonRatio = value;
                        UpdateScrollButtonBounds();
                    }
                }
            }
        }

        private int TrackBarLength => Orientation == Orientation.Vertical ? ScrollbarTrackBounds.Height : ScrollbarTrackBounds.Width;

        public bool WheelScrollLarge { get; set; }

        public string ButtonClickSound { get; set; }

        public string ScrollingSound { get; set; }

        public event EventHandler Scroll;

        public SdvScrollBar(Orientation orientation)
        {
            Orientation = orientation;
            _MaxValue = 1;
            _ScrollButtonRatio = 1f;
            SmallChange = 8;
            LargeChange = 32;
            if (Orientation == Orientation.Horizontal)
                Height = SCROLLBAR_SIZE;
            else
                Width = SCROLLBAR_SIZE;
            ButtonClickSound = "shwip";
            ScrollingSound = "shiny4";
        }

        public SdvScrollBar(Orientation orientation, bool fromContainer) :this(orientation)
        {
            IsContainerScrollBar = fromContainer;
        }

        private void OnScrollCore()
        {
            UpdateScrollButtonBounds();
            OnScroll(EventArgs.Empty);
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateScrollbarBounds();
            UpdateScrollButtonBounds();
        }

        public override Rectangle GetScreenBounds()
        {
            if (!IsContainerScrollBar)
                return base.GetScreenBounds();

            if (Parent != null)
            {
                var parentBounds = Parent.GetScreenBounds();
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

        private void AdjustMaxValue()
        {
            _MaxValue = (int)Math.Ceiling(_MaxValue / (double)LargeChange) * LargeChange;
        }

        public void SetScrollInfo(int contentSize, int viewSize)
        {
            MaxValue = contentSize - viewSize;

            if (contentSize > viewSize)
                ScrollButtonRatio = viewSize / (float)contentSize;
        }

        private void UpdateScrollbarBounds()
        {
            if (Orientation == Orientation.Vertical)
            {
                UpArrowBounds = new Rectangle(0, 0, 44, 48);
                DownArrowBounds = new Rectangle(0, Height - 48, 44, 48);
                ScrollbarTrackBounds = new Rectangle(12, UpArrowBounds.Bottom + 4, TRACK_SIZE, Height - (UpArrowBounds.Height + 4) * 2);
            }
            else
            {
                UpArrowBounds = new Rectangle(0, 0, 48, 44);
                DownArrowBounds = new Rectangle(Width - 48, 0, 48, 44);
                ScrollbarTrackBounds = new Rectangle(UpArrowBounds.Right + 4, 8, Width - (UpArrowBounds.Width + 4) * 2, TRACK_SIZE);
            }
        }

        private void UpdateScrollButtonBounds()
        {
            var scrollBtnSize = _ScrollButtonRatio != 1f ? (int)Math.Round(TrackBarLength * _ScrollButtonRatio) : SCROLLBUTTON_SIZE;
            if (Orientation == Orientation.Vertical)
            {
                ScrollbarButtonBounds = new Rectangle(ScrollbarTrackBounds.X, ScrollbarTrackBounds.Y, 24, scrollBtnSize);
                ScrollbarButtonBounds.Y += (int)((Value / (float)MaxValue) * (ScrollbarTrackBounds.Height - ScrollbarButtonBounds.Height));
            }
            else
            {
                ScrollbarButtonBounds = new Rectangle(ScrollbarTrackBounds.X, ScrollbarTrackBounds.Y, scrollBtnSize, 24);
                ScrollbarButtonBounds.X += (int)((Value / (float)MaxValue) * (ScrollbarTrackBounds.Width - ScrollbarButtonBounds.Width));
            }
        }
        
        protected override void OnDraw(SdvGraphics g)
        {
            base.OnDraw(g);
            g.DrawTextureBox(SdvImages.ScrollBarTrack, ScrollbarTrackBounds, Color.White, 4f, true);

            if (Orientation == Orientation.Vertical)
            {
                g.DrawImage(SdvImages.UpArrow, UpArrowBounds, Value > 0 ? Color.White : Color.Gray);
                g.DrawImage(SdvImages.DownArrow, DownArrowBounds, Value < MaxValue ? Color.White : Color.Gray);
                g.DrawImage(SdvImages.VScrollbarButton, ScrollbarButtonBounds, Color.White, false);
            }
            else
            {
                g.DrawImage(SdvImages.LeftArrow, UpArrowBounds, Value > 0 ? Color.White : Color.Gray);
                g.DrawImage(SdvImages.RightArrow, DownArrowBounds, Value < MaxValue ? Color.White : Color.Gray);
                //g.DrawImageRotated(SdvImages.UpArrow, UpArrowBounds, -3.14f / 2f, Color.White, 4f);
                //g.DrawImageRotated(SdvImages.DownArrow, DownArrowBounds, -3.14f / 2f, Color.White, 4f);
                g.DrawImage(SdvImages.HScrollbarButton, ScrollbarButtonBounds, Color.White, false);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Buttons == MouseButtons.Left && Enabled)
            {
                if (UpArrowBounds.Contains(e.Location))
                {
                    if (Value > 0)
                    {
                        Value -= LargeChange;
                        if (!string.IsNullOrEmpty(ButtonClickSound))
                            StardewValley.Game1.playSound(ButtonClickSound);
                    }
                }
                else if (DownArrowBounds.Contains(e.Location))
                {
                    if (Value < MaxValue)
                    {
                        Value += LargeChange;
                        if (!string.IsNullOrEmpty(ButtonClickSound))
                            StardewValley.Game1.playSound(ButtonClickSound);
                    }
                }
                else if (ScrollbarTrackBounds.Contains(e.Location))
                {
                    var valueAtPosition = GetScrollValueAtPosition(e.Location);
                    if (valueAtPosition != Value)
                    {
                        Value = valueAtPosition;
                        if (!string.IsNullOrEmpty(ScrollingSound))
                            StardewValley.Game1.playSound(ScrollingSound);
                    }
                }
            }
        }

        private bool MouseDragging = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Buttons == MouseButtons.Left && Enabled)
            {
                if (ScrollbarButtonBounds.Contains(e.Location))
                {
                    MouseDragging = true;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if(e.Buttons == MouseButtons.Left && MouseDragging)
            {
                MouseDragging = false;
            }
        }

        protected override void OnScrollWheel(int delta)
        {
            base.OnScrollWheel(delta);
            if (Enabled)
            {
                var oldValue = Value;
                Value += (WheelScrollLarge ? LargeChange : SmallChange) * Math.Sign(delta) * -1;

                if (oldValue != Value && !string.IsNullOrEmpty(ScrollingSound))
                    StardewValley.Game1.playSound(ScrollingSound);
            }
        }

        public override bool HandleScrollWheel(MouseEventArgs data)
        {
            return Enabled && Visible && ScreenBounds.Contains(data.DisplayLocation);
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
                var newValue = GetScrollValueAtPosition(e.Location);
                if (newValue != Value)
                {
                    Value = newValue;
                    if (!string.IsNullOrEmpty(ScrollingSound))
                        StardewValley.Game1.playSound(ScrollingSound);
                } 
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

        #region Sounds

        private void PlayButtonClickSound()
        {
            if(!string.IsNullOrEmpty(ButtonClickSound))
                StardewValley.Game1.playSound(ButtonClickSound);
        }
        

        #endregion
    }
}
