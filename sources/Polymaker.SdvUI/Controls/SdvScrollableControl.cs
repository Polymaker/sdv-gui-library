using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class SdvScrollableControl : SdvContainerControl
    {
        public SdvScrollBar HScrollBar { get; }

        public SdvScrollBar VScrollBar { get; }

        public bool HScrollBarVisible => HScrollBar.Visible;

        public bool VScrollBarVisible => VScrollBar.Visible;

        private SdvScrollBar[] ScrollBars => new SdvScrollBar[] { HScrollBar, VScrollBar };

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
                }
            }
        }

        public SdvScrollableControl() : base()
        {
            HScrollBar = new SdvScrollBar(Orientation.Horizontal, true);
            VScrollBar = new SdvScrollBar(Orientation.Vertical, true);
        }

        public override IEnumerable<SdvControl> GetVisibleControls()
        {
            return base.GetVisibleControls().Concat(ScrollBars.Where(s => s.Visible));
        }
    }
}
