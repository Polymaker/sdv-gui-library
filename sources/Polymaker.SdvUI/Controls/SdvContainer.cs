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
    public class SdvContainer : SdvControl, ISdvContainer, ISdvContainerCore
    {
        //private Point _ScrollOffset;

        public Point ScrollSize { get; private set; }
        
        public SdvControlCollection Controls { get; }

        public Point ScrollOffset
        {
            get => new Point(HScrollBarVisible ? HScrollBar.Value : 0, VScrollBarVisible ? VScrollBar.Value : 0);
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

        private SdvScrollBar[] ScrollBars => new SdvScrollBar[] { HScrollBar, VScrollBar };

        protected IEnumerable<SdvControl> AllControls => Controls.Concat(ScrollBars);

        IEnumerable<SdvControl> ISdvContainerCore.AllControls => AllControls;

        public SdvContainer()
        {
            Controls = new SdvControlCollection(this);
            ScrollSize = Point.Zero;

            HScrollBar = new SdvScrollBar(Orientation.Horizontal, true);
            HScrollBar.SetParent(this, true);
            VScrollBar = new SdvScrollBar(Orientation.Vertical, true);
            VScrollBar.SetParent(this, true);

            Controls.CollectionChanged += Controls_CollectionChanged;
        }

        #region Controls Add/Remove/Resize

        private void Controls_CollectionChanged(object sender, ControlsChangedEventArgs e)
        {
            if (e.ChangeType == ControlsChangedEventArgs.Action.Add)
            {
                foreach (var ctrl in e.Controls)
                    ctrl.SizeChanged += Control_SizeChanged;
            }
            else if (e.ChangeType == ControlsChangedEventArgs.Action.Remove)
            {
                foreach (var ctrl in e.Controls)
                    ctrl.SizeChanged -= Control_SizeChanged;
            }
            UpdateScrollBarsBounds();
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            UpdateScrollBarsBounds();
        }

        #endregion


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

            HScrollBar.Visible = HScrollBarVisible;
            VScrollBar.Visible = VScrollBarVisible;

            HScrollBar.MaxValue = Math.Max(ScrollSize.X - baseClientRect.Width, 1);
            VScrollBar.MaxValue = Math.Max(ScrollSize.Y - baseClientRect.Height, 1);

            HScrollBar.SetBounds(0, Height - HScrollBar.Height, Width - (VScrollBarVisible ? VScrollBar.Width : 0), 0, ControlBounds.Y | ControlBounds.Width);
            VScrollBar.SetBounds(Width - VScrollBar.Width, 0, 0, Height - (HScrollBarVisible ? HScrollBar.Height : 0), ControlBounds.X | ControlBounds.Height);
        }

        protected override void OnDraw(SpriteBatch b)
        {
            base.OnDraw(b);
            var displayRect = GetDisplayRectangle();
            //var clientRect = GetClientRectangle();
            displayRect.Width -= VScrollBarVisible ? VScrollBar.Width : 0;
            displayRect.Height -= HScrollBarVisible ? HScrollBar.Height : 0;

            foreach (var control in ScrollBars.Where(s => s.Visible))
                control.PerformDraw(b);

            using (var clip = new GraphicClip(b, displayRect))
            {
                foreach (var control in Controls.Where(c => c.Visible))
                    if (!(control is ISdvContainer))
                        control.PerformDraw(b);
            }

            foreach (var control in Controls.Where(c => c.Visible))
                if (control is ISdvContainer)
                    control.PerformDraw(b);
        }

        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    foreach(var scrollBar in ScrollBars)
        //    {
        //        if (scrollBar.Visible && scrollBar.Bounds.Contains(e.Location))
        //        {
        //            ((ISdvCoreEvents)scrollBar).OnMouseDown(new MouseEventArgs(scrollBar.PointToLocal(e.DisplayLocation), e.DisplayLocation, e.Button));
        //            return;
        //        }
        //    }

        //    base.OnMouseDown(e);
        //}

        //protected override void OnMouseUp(MouseEventArgs e)
        //{
        //    foreach (var scrollBar in ScrollBars)
        //    {
        //        if (scrollBar.Visible && scrollBar.Bounds.Contains(e.Location))
        //        {
        //            ((ISdvCoreEvents)scrollBar).OnMouseUp(new MouseEventArgs(scrollBar.PointToLocal(e.DisplayLocation), e.DisplayLocation, e.Button));
        //            return;
        //        }
        //    }

        //    base.OnMouseUp(e);
        //}

        public override bool CaptureMouseWheel(int x, int y)
        {
            foreach(var control in Controls)
            {
                var localPt = PointToControl(x, y, control);
                if (control.CaptureMouseWheel(localPt.X, localPt.Y))
                    return true;
            }

            return VScrollBar.Visible;
        }

        protected override void OnScrollWheel(int delta)
        {
            var curPos = CursorPosition;
            var controlAtCursor = GetControlAtPosition(curPos);
            if (controlAtCursor != null && controlAtCursor != this && controlAtCursor.CaptureMouseWheel(curPos.X, curPos.Y))
            {
                ((ISdvCoreEvents)controlAtCursor).OnScrollWheel(delta);
            }
            else if(VScrollBarVisible)
            {
                ((ISdvCoreEvents)VScrollBar).OnScrollWheel(delta);
            }
        }

        private Point PointToControl(int x, int y, SdvControl control)
        {
            return new Point(x - control.X, y - control.Y);
        }

        public SdvControl GetControlAtPosition(int x, int y)
        {
            return SdvForm.GetControlAtPosition(this, x, y);
        }

        public SdvControl GetControlAtPosition(Point position)
        {
            return SdvForm.GetControlAtPosition(this, position.X, position.Y);
        }
    }
}
