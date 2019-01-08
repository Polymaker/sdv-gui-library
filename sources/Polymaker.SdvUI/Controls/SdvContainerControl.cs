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
        public SdvControlCollection Controls { get; }

        public SdvContainerControl()
        {
            Controls = new SdvControlCollection(this);
        }

        public virtual IEnumerable<SdvControl> GetVisibleControls()
        {
            return Controls.Where(c => c.Visible);
        }

        protected override Point GetPreferredSize()
        {
            var newSize = Point.Zero;

            foreach (var ctrl in Controls.Where(c => c.Visible))
            {
                newSize.X = Math.Max(ctrl.Bounds.Right, newSize.X);
                newSize.Y = Math.Max(ctrl.Bounds.Bottom, newSize.Y);
            }

            return new Point(newSize.X + Padding.Horizontal, newSize.Y + Padding.Vertical);
        }

        public SdvControl GetControlAtPosition(int x, int y, bool localPoint = false)
        {
            return GetControlAtPosition(this, x, y, localPoint);
        }

        public SdvControl GetControlAtPosition(Point position, bool localPoint = false)
        {
            return GetControlAtPosition(this, position.X, position.Y, localPoint);
        }

        public override bool ForwardScrollWheel(MouseEventArgs data)
        {
            var localPt = PointToLocal(data.DisplayLocation);
            var pointInClient = ClientRectangle.Contains(localPt);

            foreach (var control in GetVisibleControls())
            {
                if (!(pointInClient || !Controls.Contains(control)))
                    continue;
                var localData = data.ToLocal(control);
                if (control.HandleScrollWheel(localData))
                {
                    control.PerformScrollWheel(data.Delta);
                    return true;
                }
                else if (control.ForwardScrollWheel(localData))
                    return true;
            }

            return base.ForwardScrollWheel(data);
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

            foreach (var ctrl in container.GetVisibleControls())
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
