using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class SdvContainerControl
    {
        public SdvControlCollection Controls { get; }

        public SdvContainerControl()
        {
            Controls = new SdvControlCollection((ISdvContainer)this);
        }

        public virtual IEnumerable<SdvControl> GetVisibleControls()
        {
            return Controls.Where(c => c.Visible);
        }
    }
}
