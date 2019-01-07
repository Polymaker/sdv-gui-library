using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polymaker.SdvUI.Drawing;

namespace Polymaker.SdvUI.Controls.GridControl
{
    public class SdvGridControl : SdvControl
    {
        private System.Collections.IList _DataSource;

        public IList DataSource { get => _DataSource; set => _DataSource = value; }

        protected override void OnDraw(SdvGraphics g)
        {
            base.OnDraw(g);
        }


    }
}
