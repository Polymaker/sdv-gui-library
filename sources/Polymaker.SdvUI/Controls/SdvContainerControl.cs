using Microsoft.Xna.Framework;
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

            if (ClientRectangle.Contains(localPt))
            {
                foreach (var control in GetVisibleControls())
                {
                    var localData = data.ToLocal(control);
                    if (control.HandleScrollWheel(localData))
                    {
                        control.PerformScrollWheel(data.Delta);
                        break;
                    }
                    else if (control.ForwardScrollWheel(localData))
                        break;
                }
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

            foreach (var ctrl in container.GetVisibleControls())
            {
                if (ctrl.Visible && ctrl.DisplayRectangle.Contains(x, y))
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
    }
}
