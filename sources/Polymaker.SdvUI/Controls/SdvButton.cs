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

            var mouseOver = DisplayRectangle.Contains(CursorPosition);
            var pressed = Focused && Cursor.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            var imgColor = (pressed || !Enabled) ? new Color(110, 110, 110) : (mouseOver ? Color.LightGray : Color.White);

            g.DrawTextureBox(
                SdvImages.ButtonTexture, 
                DisplayRectangle,
                imgColor, 
                4f);
        }
    }
}
