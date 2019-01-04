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

        protected override void OnDrawBackground(SpriteBatch b)
        {
            base.OnDrawBackground(b);
            b.DrawTextureBox(SdvImages.ButtonTexture, GetDisplayRectangle(), Color.White, 4f);
        }

        protected override void OnDrawBackground2(SdvGraphics g)
        {
            base.OnDrawBackground2(g);
            g.DrawTextureBox(SdvImages.ButtonTexture, new Rectangle(0, 0, Width, Height), Color.White, 4f);
        }
    }
}
