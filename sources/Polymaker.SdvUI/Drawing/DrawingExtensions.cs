using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polymaker.SdvUI.Controls;
using Polymaker.SdvUI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI
{
    public static class DrawingExtensions
    {
        #region Draw Imge

        public static void DrawImage(this SpriteBatch b, SdvImage image, Vector2 position)
        {
            DrawImage(b, image, new Rectangle((int)position.X, (int)position.Y, (int)image.Size.X, (int)image.Size.Y));
        }

        public static void DrawImage(this SpriteBatch b, SdvImage image, Vector2 position, float scale)
        {
            DrawImage(b, image, new Rectangle((int)position.X, (int)position.Y, (int)(image.SourceSize.X * scale), (int)(image.SourceSize.Y * scale)));
        }

        public static void DrawImage(this SpriteBatch b, SdvImage image, Rectangle destination)
        {
            DrawImage(b, image, destination, Color.White, false);
        }

        public static void DrawImage(this SpriteBatch b, SdvImage image, Rectangle destination, Color color, bool withShadow = false)
        {
            var scale = new Vector2(destination.Width / image.SourceSize.X, destination.Height / image.SourceSize.Y);
            if (withShadow)
            {
                b.Draw(image.Texture, new Vector2(destination.X - 4f, destination.Y + 4f), image.SourceRect,
                    Color.Black * 0.35f, 0, Vector2.Zero, scale, SpriteEffects.None, (destination.Y / 10000f) - 0.0001f);
            }

            b.Draw(image.Texture, new Vector2(destination.X, destination.Y), image.SourceRect, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public static void DrawImageUnstreched(this SpriteBatch b, SdvImage image, Rectangle destination, Color color, bool withShadow = false)
        {

        }

        public static void DrawImageUnscaled(this SpriteBatch b, SdvImage image, Rectangle destination, Color color)
        {
            b.Draw(image.Texture,
                    new Vector2(destination.X + (float)(image.SourceRect.Width / 2), destination.Y + (float)(image.SourceRect.Height / 2)), image.SourceRect,
                    color, 0f, new Vector2(image.SourceRect.Width / 2, image.SourceRect.Height / 2), 1f, SpriteEffects.None, 0.86f + destination.Y / 20000f);
        }


        public static void DrawImageRotated(this SpriteBatch b, SdvImage image, Rectangle destination, float rotation, Color color, float scale = 1f, bool withShadow = false)
        {
            if (withShadow)
            {
                Utility.drawWithShadow(b, image.Texture,
                    new Vector2(destination.X + image.SourceRect.Width / 2 * scale, destination.Y + image.SourceRect.Height / 2 * scale),
                    image.SourceRect, color, rotation, new Vector2(image.SourceRect.Width / 2, image.SourceRect.Height / 2), scale, false, 0.86f + destination.Y / 20000f, -1, -1, 0.35f);
            }
            else
            {
                b.Draw(image.Texture,
                    new Vector2(destination.X + image.SourceRect.Width / 2 * scale, destination.Y + image.SourceRect.Height / 2 * scale), image.SourceRect,
                    color, rotation, new Vector2(image.SourceRect.Width / 2, image.SourceRect.Height / 2), scale, SpriteEffects.None, 0.86f + destination.Y / 20000f);
            }
        }

        #endregion

        #region TextureBox

        public static void DrawTextureBox(this SpriteBatch b, SdvImage image, Vector2 position)
        {
            DrawTextureBox(b, image, new Rectangle((int)position.X, (int)position.Y, (int)image.Size.X, (int)image.Size.Y));
        }

        public static void DrawTextureBox(this SpriteBatch b, SdvImage image, Rectangle destination)
        {
            var computedScale = 1f;
            if (image.Scale == 1f && (image.SourceRect.Width != destination.Width || image.SourceRect.Height != destination.Height))
            {
                var minSize = Math.Min(image.Size.X, image.Size.Y);
                computedScale = (float)Math.Round(minSize / 8f);
            }
            DrawTextureBox(b, image, destination, Color.White, computedScale, false);
        }

        public static void DrawTextureBox(this SpriteBatch b, SdvImage image, Rectangle destination, float scale)
        {
            DrawTextureBox(b, image, destination, Color.White, scale, false);
        }

        public static void DrawTextureBox(this SpriteBatch b, SdvImage image, Rectangle destination, Color color/* = default(Color)*/, float scale = 1f, bool withShadow = false)
        {
            if (color == default(Color))
                color = Color.White;

            IClickableMenu.drawTextureBox(b, image.Texture, image.SourceRect, destination.X, destination.Y, destination.Width, destination.Height, color, scale, withShadow);
        }

        #endregion

        #region Text

        public static void DrawString(this SpriteBatch b, string text, SdvFont font, Color color, Vector2 pos)
        {
            if (font.Bold)
            {
                //todo: draw shadow if combined
                Utility.drawBoldText(b, text, font.Sprite, pos, color, font.Scale);
            }
            else if (font.DrawShadow)
                Utility.drawTextWithShadow(b, text, font.Sprite, pos, color, font.Scale);
            else
                b.DrawString(font.Sprite, text, pos, color, 0f, Vector2.Zero, font.Scale, SpriteEffects.None, 0f);
        }

        public static void DrawString(this SpriteBatch b, string text, SdvFont font, Color color, Rectangle rect)
        {
            DrawString(b, text, font, color, new Vector2(rect.X, rect.Y));
        }

        public static void DrawString(this SpriteBatch b, string text, SdvFont font, Color color, Rectangle rect, ContentAlignment textAlign)
        {
            var textSize = font.MeasureString(text);
            var correctedBounds = LayoutHelper.GetAlignedBounds(rect, textSize, textAlign);
            DrawString(b, text, font, color, new Vector2(correctedBounds.X, correctedBounds.Y));
        }

        #endregion

    }
}
