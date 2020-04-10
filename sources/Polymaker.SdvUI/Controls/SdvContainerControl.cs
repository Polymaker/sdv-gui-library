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
    public class SdvContainerControl : SdvControl, ISdvContainer
    {
        private SdvControl _ActiveControl;

        public SdvControl ActiveControl
        {
            get => _ActiveControl;
            set => SetActiveControl(value);
        }

        //public SdvControl ActiveControl
        //{
        //    get => ParentForm?.ActiveControl;
        //    set => SetActiveControl(value);
        //}

        public SdvControlCollection Controls { get; }

        public bool ContainsFocus
        {
            get
            {
                return ActiveControl?.Focused ?? false;
                //return GetVisibleControls().Any(c => c.Focused || ((c as SdvContainerControl)?.ContainsFocus ?? false));
            }
        }

        public SdvContainerControl()
        {
            Controls = new SdvControlCollection(this);
        }

        public virtual IEnumerable<SdvControl> GetVisibleControls()
        {
            return Controls.Where(c => c.Visible);
        }

        public override Point GetPreferredSize()
        {
            var newSize = Point.Zero;

            foreach (var ctrl in Controls.Where(c => c.Visible))
            {
                newSize.X = Math.Max(ctrl.Bounds.Right, newSize.X);
                newSize.Y = Math.Max(ctrl.Bounds.Bottom, newSize.Y);
            }

            return new Point(newSize.X + Padding.Horizontal, newSize.Y + Padding.Vertical);
        }

        public virtual bool Contains(SdvControl control)
        {
            return Controls.Contains(control);
        }

        public SdvControl GetControlAtPosition(int x, int y, bool localPoint = false)
        {
            return GetControlAtPosition(this, x, y, localPoint);
        }

        public SdvControl GetControlAtPosition(Point position, bool localPoint = false)
        {
            return GetControlAtPosition(this, position.X, position.Y, localPoint);
        }

        internal static SdvControl GetControlAtPosition(ISdvContainer container, int x, int y, bool localPoint = false)
        {
            
            if (localPoint)
            {
                var pt = container.PointToDisplay(new Point(x, y));
                x = pt.X;
                y = pt.Y;
            }
            Point localPt = container.PointToLocal(new Point(x, y));
            var pointInClient = container.ClientRectangle.Contains(localPt);
            var visibleControls = container.GetVisibleControls();

            foreach (var ctrl in visibleControls.Reverse())
            {
                if (ctrl.Visible && ctrl.ScreenBounds.Contains(x, y) && (pointInClient || !container.Controls.Contains(ctrl)))
                {
                    if (ctrl is ISdvContainer childContainer)
                        return GetControlAtPosition(childContainer, x, y);

                    return ctrl;
                }
            }

            if (container is SdvControl control)
                return control;

            return null;
        }

        internal void SetActiveControl(SdvControl value)
        {
            if (value == _ActiveControl)
            {
                if (value == null || value.Focused)
                    return;
            }

            if (value != null && !Contains(value))
                throw new ArgumentException("The specified control is not owned by this container.");

            SetActiveControlInternal(value);
        }

        internal bool SetActiveControlInternal(SdvControl value)
        {
            if (Parent is SdvForm form)
            {
                if (form.SetActiveControlInternal(value))
                {
                    _ActiveControl = value;
                    return true;
                }
            }
            else if (Parent is SdvContainerControl container)
            {
                if (container.SetActiveControlInternal(value))
                {
                    _ActiveControl = value;
                    return true;
                }
            }
            return false;
        }

        protected override void OnDraw(SdvGraphics g)
        {
            //using (var clip = new GraphicClip(g.SB, DisplayRectangle))
            //{
            //    if (clip.Invisible)
            //        return;

                foreach (var control in Controls.Where(c => c.Visible))
                {
                    control.PerformDraw(g.SB);
                }
            //}
        }
    }
}
