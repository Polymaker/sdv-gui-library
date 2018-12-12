using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class SdvGameMenuForm : SdvForm
    {
        //public override Padding Padding { get => GameMenuPadding; set { } }

        public SdvGameMenuForm()
        {
            Padding = GameMenuPadding;
        }

        public SdvGameMenuForm(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
        {
            Padding = GameMenuPadding;
        }
    }
}
