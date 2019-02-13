using Microsoft.Xna.Framework;
using Polymaker.SdvUI.Drawing;
using Polymaker.SdvUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class SdvScrollableControl : SdvContainerControl, IScrollableContainer
    {
        private Point _MinScrollSize;

        public SdvScrollBar HScrollBar { get; }

        public SdvScrollBar VScrollBar { get; }

        public bool HScrollVisible => HScrollBar.Visible;

        public bool VScrollVisible => VScrollBar.Visible;

        private SdvScrollBar[] ScrollBars => new SdvScrollBar[] { HScrollBar, VScrollBar };

        public Point ScrollOffset
        {
            get => new Point(HScrollVisible ? HScrollBar.Value : 0, VScrollVisible ? VScrollBar.Value : 0);
            set
            {
                if (value != ScrollOffset)
                {
                    if (HScrollVisible)
                        HScrollBar.Value = value.X;
                    if (VScrollVisible)
                        VScrollBar.Value = value.Y;
                }
            }
        }

        public Point MinScrollSize
        {
            get => _MinScrollSize;
            set
            {
                value.X = value.X >= 0 ? value.X : 0;
                value.Y = value.Y >= 0 ? value.Y : 0;
                if (value != _MinScrollSize)
                {
                    _MinScrollSize = value;
                    UpdateScrollBars();
                }
            }
        }

        public Point ScrollSize { get; private set; }

        public SdvScrollableControl() : base()
        {
            ScrollSize = Point.Zero;
            _MinScrollSize = Point.Zero;
            HScrollBar = new SdvScrollBar(Orientation.Horizontal, true);
            HScrollBar.SetParent(this, true);
            VScrollBar = new SdvScrollBar(Orientation.Vertical, true);
            VScrollBar.SetParent(this, true);
            HScrollBar.Scroll += ScrollBars_Scroll;
            VScrollBar.Scroll += ScrollBars_Scroll;
            Controls.CollectionChanged += Controls_CollectionChanged;
        }

        private void Controls_CollectionChanged(object sender, ControlsChangedEventArgs e)
        {
            foreach(var ctrl in e.Controls)
            {
                if(e.ChangeType == ControlsChangedEventArgs.Action.Add)
                    ctrl.SizeChanged += Controls_SizeChanged;
                else
                    ctrl.SizeChanged -= Controls_SizeChanged;
            }
            UpdateScrollBars();
        }

        private void Controls_SizeChanged(object sender, EventArgs e)
        {
            UpdateScrollBars();
        }

        private void ScrollBars_Scroll(object sender, EventArgs e)
        {
            ScrollChangedCore();
        }

        protected override void OnScrollWheel(int delta)
        {
            base.OnScrollWheel(delta);

            if(Enabled && (VScrollVisible || HScrollVisible))
            {
                (VScrollVisible ? VScrollBar : HScrollBar).ProcessEvent(Events.SdvEvents.ScrollWheel, delta);
            }
        }

        public override bool HandleScrollWheel(MouseEventArgs data)
        {
            return Enabled && (VScrollVisible || HScrollVisible) && DisplayRectangle.Contains(data.Location);
        }

        public override Rectangle GetClientRectangle()
        {
            return new Rectangle(Padding.Left, Padding.Top,
                Width - (VScrollVisible ? VScrollBar.Width : 0),
                Height - (HScrollVisible ? HScrollBar.Height : 0));
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, ControlBounds specifiedBounds)
        {
            width = Math.Max(width, Padding.Horizontal + VScrollBar.Width);
            height = Math.Max(height, Padding.Vertical + HScrollBar.Height);
            base.SetBoundsCore(x, y, width, height, specifiedBounds);
            UpdateScrollBars();
        }

        private void ScrollChangedCore()
        {
            foreach(var ctrl in Controls)
            {
                ctrl.Invalidate();
            }
        }

        private void CalculateScrollSize()
        {
            var newSize = Point.Zero;

            foreach (var ctrl in Controls.Where(c => c.Visible))
            {
                newSize.X = Math.Max(ctrl.Bounds.Right, newSize.X);
                newSize.Y = Math.Max(ctrl.Bounds.Bottom, newSize.Y);
            }

            //if (newSize.X > 0 || newSize.Y > 0)
            //{
            //    //newSize.X += Padding.Horizontal;
            //    //newSize.Y += Padding.Vertical;
            //    newSize.X += Padding.Right;
            //    newSize.Y += Padding.Bottom;
            //}

            newSize.X = Math.Max(newSize.X, MinScrollSize.X);
            newSize.Y = Math.Max(newSize.Y, MinScrollSize.Y);

            ScrollSize = newSize;
        }

        private void UpdateScrollBars()
        {
            HScrollBar.Visible = false;
            VScrollBar.Visible = false;

            if (Width > 0 && Height > 0)
            { 
                CalculateScrollSize();

                var baseClientRect = new Rectangle(Padding.Left, Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical);

                if (baseClientRect.Height < ScrollSize.Y)
                {
                    VScrollBar.Visible = true;
                    baseClientRect.Width -= VScrollBar.Width;
                }

                if (baseClientRect.Width < ScrollSize.X)
                {
                    HScrollBar.Visible = true;
                    baseClientRect.Height -= HScrollBar.Height;

                    if (!VScrollBar.Visible && baseClientRect.Height < ScrollSize.Y)
                    {
                        VScrollBar.Visible = true;
                        baseClientRect.Width -= VScrollBar.Width;
                    }
                }


                HScrollBar.MaxValue = Math.Max(ScrollSize.X - baseClientRect.Width, 1);
                VScrollBar.MaxValue = Math.Max(ScrollSize.Y - baseClientRect.Height, 1);

                //HScrollBar.SetScrollInfo(ScrollSize.X, baseClientRect.Width);
                //VScrollBar.SetScrollInfo(ScrollSize.Y, baseClientRect.Height);

                HScrollBar.SetBounds(0, Height - HScrollBar.Height, Width - (VScrollBar.Visible ? VScrollBar.Width : 0), 0, ControlBounds.Y | ControlBounds.Width);
                VScrollBar.SetBounds(Width - VScrollBar.Width, 0, 0, Height - (HScrollBar.Visible ? HScrollBar.Height : 0), ControlBounds.X | ControlBounds.Height);
            }
        }

        public override IEnumerable<SdvControl> GetVisibleControls()
        {
            return base.GetVisibleControls().Concat(ScrollBars.Where(s => s.Visible));
        }

        protected override void OnDraw(SdvGraphics g)
        {
            var displayRect = ScreenBounds;
            displayRect.Width -= VScrollVisible ? VScrollBar.Width : 0;
            displayRect.Height -= HScrollVisible ? HScrollBar.Height : 0;

            foreach (var control in ScrollBars.Where(s => s.Visible))
                control.PerformDraw(g.SB);

            using (var clip = new GraphicClip(g.SB, displayRect))
            {
                if (clip.Invisible)
                    return;

                foreach (var control in Controls.Where(c => c.Visible))
                {
                    //if (!(control is SdvContainerControl))
                    //    control.PerformDraw(g.SB);
                    control.PerformDraw(g.SB);
                }
            }
        }
    }
}
