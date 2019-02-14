using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public abstract class SdvPopupBase : SdvContainerControl
    {
        public int ZIndex { get; set; }

        public SdvPopupBase()
        {
            
            ZIndex = 100;
        }

        public void Show(SdvForm window)
        {

        }
    }
}
