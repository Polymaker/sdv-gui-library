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
        public SdvImage ButtonTexture { get; set; }

        public bool IsTextureBox { get; set; }

        public SdvButton() : base()
        {
            Padding = new Padding(16, 8, 16, 8);
            ButtonTexture = SdvImages.ButtonTexture;
            ButtonTexture.Scale = 4f;
            IsTextureBox = true;
        }

        protected override void OnDrawBackground(SdvGraphics g)
        {
            base.OnDrawBackground(g);

            if (ButtonTexture != null)
            {
                var mouseOver = DisplayRectangle.Contains(CursorPosition);
                var pressed = Focused && Cursor.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                var imgColor = pressed ? new Color(110, 110, 110) : (mouseOver ? Color.LightGray : Color.White);

                if (!Enabled)
                    imgColor = new Color(150, 150, 150, 220);

                if (IsTextureBox)
                {
                    g.DrawTextureBox(
                        ButtonTexture,
                        DisplayRectangle,
                        imgColor,
                        ButtonTexture.Scale);
                }
                else
                {
                    g.DrawImage(ButtonTexture, DisplayRectangle, imgColor);
                }
            }
        }
    }
}
