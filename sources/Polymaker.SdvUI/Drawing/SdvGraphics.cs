using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polymaker.SdvUI.Controls;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Drawing
{
    public class SdvGraphics : IDisposable
    {
        public Point Offset { get; set; }
        public SpriteBatch SB { get; private set; }

        internal SdvGraphics()
        {

        }

        public SdvGraphics(SpriteBatch sb, Point translation)
        {
            SB = sb;
            Offset = translation;
        }

        #region Misc

        public void FillRect(Color color, Rectangle rect)
        {
            FillRect(color, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillRect(Color color, int x, int y, int w, int h)
        {
            SB.Draw(Game1.staminaRect, new Rectangle(x + Offset.X, y + Offset.Y, w, h), color);
        }

        #endregion

        #region Images Drawing

        public void DrawImage(SdvImage image, int x, int y)
        {
            DrawImage(image, x, y, (int)image.Size.X, (int)image.Size.Y);
        }

        public void DrawImage(SdvImage image, Vector2 pos)
        {
            DrawImage(image, (int)pos.X, (int)pos.Y);
        }

        public void DrawImage(SdvImage image, Point pos)
        {
            DrawImage(image, pos.X, pos.Y);
        }

        public void DrawImage(SdvImage image, int x, int y, int w, int h)
        {
            DrawImage(image, new Rectangle(x, y, w, h));
        }

        public void DrawImage(SdvImage image, Rectangle destination)
        {
            destination.Offset(Offset);
            DrawingExtensions.DrawImage(SB, image, destination);
        }

        public void DrawImage(SdvImage image, Rectangle destination, float scale)
        {
            destination.Offset(Offset);
            DrawingExtensions.DrawImage(SB, image, destination, scale);
        }

        public void DrawImage(SdvImage image, Rectangle destination, Color color/* = default(Color)*/, float scale = 1f, bool withShadow = false)
        {
            destination.Offset(Offset);
            DrawingExtensions.DrawImage(SB, image, destination, color, scale, withShadow);
        }

        public void DrawImageRotated(SdvImage image, Rectangle destination, float rotation, Color color, float scale = 1f, bool withShadow = false)
        {
            destination.Offset(Offset);
            DrawingExtensions.DrawImageRotated(SB, image, destination, rotation, color, scale, withShadow);
        }

        #endregion

        #region TextureBox

        //public void DrawTextureBox(SdvImage image, Vector2 position)
        //{
        //    DrawingExtensions.DrawTextureBox(SB, image, position);
        //}

        //public void DrawTextureBox(SdvImage image, Rectangle destination)
        //{
        //    DrawingExtensions.DrawTextureBox(SB, image, destination);
        //}

        public void DrawTextureBox(SdvImage image, Rectangle destination, float scale)
        {
            DrawTextureBox(image, destination, Color.White, scale, false);
        }

        public void DrawTextureBox(SdvImage image, Rectangle destination, Color color/* = default(Color)*/, float scale = 1f, bool withShadow = false)
        {
            destination.Offset(Offset);
            DrawingExtensions.DrawTextureBox(SB, image, destination, color, scale, withShadow);
        }

        #endregion


        #region Text Drawing

        public void DrawString(string text, SdvFont font, Color color, int x, int y)
        {
            DrawingExtensions.DrawString(SB, text, font, color, new Vector2(x + Offset.X, y + Offset.Y));
        }

        public void DrawString(string text, SdvFont font, Color color, Vector2 pos)
        {
            DrawString(text, font, color, (int)pos.X, (int)pos.Y);
        }

        public void DrawString(string text, SdvFont font, Color color, Point pos)
        {
            DrawString(text, font, color, pos.X, pos.Y);
        }

        public void DrawString(string text, SdvFont font, Color color, Rectangle rect)
        {
            rect.Offset(Offset);
            DrawingExtensions.DrawString(SB, text, font, color, rect);
        }

        public void DrawString(string text, SdvFont font, Color color, Rectangle rect, ContentAlignment textAlign)
        {
            rect.Offset(Offset);
            DrawingExtensions.DrawString(SB, text, font, color, rect, textAlign);
        }

        #endregion

        public void Dispose()
        {
            
        }
    }
}
