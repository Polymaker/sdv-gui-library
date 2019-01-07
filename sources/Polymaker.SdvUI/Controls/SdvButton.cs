using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polymaker.SdvUI.Drawing;
using StardewValley.Menus;

namespace Polymaker.SdvUI.Controls
{
    public class SdvButton : SdvLabel
    {
        public SdvButton() : base()
        {
            Padding = new Padding(16, 8, 16, 8);
        }

        protected override void OnDrawBackground(SdvGraphics g)
        {
            base.OnDrawBackground(g);
            g.DrawTextureBox(SdvImages.ButtonTexture, new Rectangle(0, 0, Width, Height), Color.White, 4f);
        }
    }
}
