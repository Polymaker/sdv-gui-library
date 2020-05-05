using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polymaker.SdvUI.Drawing;

namespace Polymaker.SdvUI.Controls
{
    public class ComboBoxDropDown : SdvPopupBase
    {
        public SdvComboBox ComboBox { get; }

        public ComboBoxDropDown(SdvComboBox comboBox)
        {
            ComboBox = comboBox;
        }

        protected override void OnDraw(SdvGraphics g)
        {
            base.OnDraw(g);
        }
    }
}
