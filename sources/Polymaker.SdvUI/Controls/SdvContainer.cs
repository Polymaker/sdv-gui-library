using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polymaker.SdvUI.Utilities;

namespace Polymaker.SdvUI.Controls
{
    public class SdvContainer : SdvControl, ISdvContainer
    {
        //private Point _ScrollOffset;

        public Point ScrollSize { get; private set; }
        
        public SdvControlCollection Controls { get; }

        public Point ScrollOffset
        {
            get => new Point(HScrollBarVisible ? HScrollBar.Value * -1 : 0, VScrollBarVisible ? VScrollBar.Value * -1 : 0);
            set
            {
                if (value != ScrollOffset)
                {
                    if (HScrollBarVisible)
                        HScrollBar.Value = value.X;
                    if (VScrollBarVisible)
                        VScrollBar.Value = value.Y;
                    //_ScrollOffset = value;
                }
            }
        }

        public SdvScrollBar HScrollBar { get; }
        public SdvScrollBar VScrollBar { get; }

        public bool HScrollBarVisible { get; private set; }

        public bool VScrollBarVisible { get; private set; }

        public SdvContainer()
        {
            Controls = new SdvControlCollection(this);
            ScrollSize = Point.Zero;

            HScrollBar = new SdvScrollBar(Orientation.Horizontal, true);
            HScrollBar.SetParent(this, true);
            //HScrollBar.Scroll += HScrollBar_Scroll;
            VScrollBar = new SdvScrollBar(Orientation.Vertical, true);
            VScrollBar.SetParent(this, true);
            //VScrollBar.Scroll += VScrollBar_Scroll;
            Controls.ControlAdded += Controls_ControlAdded;
            Controls.ControlRemoved += Controls_ControlRemoved;
        }

        private void VScrollBar_Scroll(object sender, EventArgs e)
        {
            
        }

        private void HScrollBar_Scroll(object sender, EventArgs e)
        {
            
        }

        private void Controls_ControlAdded(object sender, ControlsChangedEventArgs e)
        {
            e.Control.SizeChanged += Control_SizeChanged;
            UpdateScrollBarsBounds();
        }

        private void Controls_ControlRemoved(object sender, ControlsChangedEventArgs e)
        {
            e.Control.SizeChanged -= Control_SizeChanged;
            UpdateScrollBarsBounds();
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            UpdateScrollBarsBounds();
        }

        public Rectangle GetClientRectangle()
        {
            return new Rectangle(Padding.Left, Padding.Top,
                Width - Padding.Horizontal - (VScrollBarVisible ? VScrollBar.Width : 0),
                Height - Padding.Vertical - (HScrollBarVisible ? HScrollBar.Height : 0));
        }

        protected override Point GetPreferredSize()
        {
            var minSize = new Point(Padding.Horizontal + 16, Padding.Vertical + 16);
            if (VScrollBarVisible)
                minSize.X += VScrollBar.Width;
            if (HScrollBarVisible)
                minSize.Y += HScrollBar.Height;
            return minSize;
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, ControlBounds specifiedBounds)
        {
            width = Math.Max(width, Padding.Horizontal);
            height = Math.Max(height, Padding.Vertical);

            base.SetBoundsCore(x, y, width, height, specifiedBounds);
            UpdateScrollBarsBounds();
        }

        private void RecalculateScrollSize()
        {
            var newSize = Point.Zero;

            foreach (var ctrl in Controls)
            {
                newSize.X = Math.Max(ctrl.Bounds.Right, newSize.X);
                newSize.Y = Math.Max(ctrl.Bounds.Bottom, newSize.Y);
            }

            if(newSize.X > 0 || newSize.Y > 0)
            {
                newSize.X += Padding.Horizontal;
                newSize.Y += Padding.Vertical;
            }

            if (newSize != ScrollSize)
            {
                ScrollSize = newSize;
            }
        }

        private void UpdateScrollBarsBounds()
        {
            VScrollBarVisible = false;
            HScrollBarVisible = false;

            if (Width <= 0 || Height <= 0)
                return;

            RecalculateScrollSize();

            var baseClientRect = new Rectangle(Padding.Left, Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical);

            if (baseClientRect.Height < ScrollSize.Y)
            {
                VScrollBarVisible = true;
                baseClientRect.Width -= VScrollBar.Width;
            }

            if (baseClientRect.Width < ScrollSize.X)
            {
                HScrollBarVisible = true;
                baseClientRect.Height -= HScrollBar.Height;

                if (!VScrollBarVisible && baseClientRect.Height < ScrollSize.Y)
                {
                    VScrollBarVisible = true;
                    baseClientRect.Width -= VScrollBar.Width;
                }
            }

            HScrollBar.MaxValue = Math.Max(ScrollSize.X - baseClientRect.Width, 1);
            VScrollBar.MaxValue = Math.Max(ScrollSize.Y - baseClientRect.Height, 1);

            HScrollBar.SetBounds(0, Height - HScrollBar.Height, Width - (VScrollBarVisible ? VScrollBar.Width : 0), 0, ControlBounds.Y | ControlBounds.Width);
            VScrollBar.SetBounds(Width - VScrollBar.Width, 0, 0, Height - (HScrollBarVisible ? HScrollBar.Height : 0), ControlBounds.X | ControlBounds.Height);
        }

        protected override void OnDraw(SpriteBatch b)
        {
            base.OnDraw(b);
            var displayRect = GetDisplayRectangle();
            var clientRect = GetClientRectangle();
            clientRect.X += displayRect.X;
            clientRect.Y += displayRect.Y;

            if (HScrollBarVisible)
                HScrollBar.PerformDraw(b);

            if (VScrollBarVisible)
                VScrollBar.PerformDraw(b);

            foreach (var control in Controls)
                control.PerformDraw(b);

            //using (var clip = new GraphicClip(b, clientRect))
            //{
            //    foreach (var control in Controls)
            //        if (!(control is ISdvContainer))
            //            control.PerformDraw(b);
            //}

            //foreach (var control in Controls)
            //    if (control is ISdvContainer)
            //        control.PerformDraw(b);
        }

        protected internal override void OnLeftClick(Point pos)
        {
            var allControls = Controls.ToList();
            if (HScrollBarVisible)
                allControls.Add(HScrollBar);
            if (VScrollBarVisible)
                allControls.Add(VScrollBar);

            foreach (var ctrl in allControls)
            {
                if(ctrl.Bounds.Contains(pos))
                {
                    var localPt = new Point(pos.X - ctrl.X, pos.Y - ctrl.Y);
                    ctrl.OnLeftClick(localPt);
                    return;
                }
            }
            base.OnLeftClick(pos);
        }
    }
}
