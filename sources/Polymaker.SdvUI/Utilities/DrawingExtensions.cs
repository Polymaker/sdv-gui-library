using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public static void DrawImage(this SpriteBatch b, SdvImage image, Rectangle destination)
        {
            var computedScale = 1f;
            if (image.Scale == 1f && (image.SourceRect.Width != destination.Width || image.SourceRect.Height != destination.Height))
            {
                var minSize = Math.Min(image.Size.X, image.Size.Y);
                computedScale = (float)Math.Round(minSize / 8f);
            }
            DrawImage(b, image, destination, Color.White, computedScale, false);
        }

        public static void DrawImage(this SpriteBatch b, SdvImage image, Rectangle destination, float scale)
        {
            DrawImage(b, image, destination, Color.White, scale, false);
        }

        public static void DrawImage(this SpriteBatch b, SdvImage image, Rectangle destination, Color color/* = default(Color)*/, float scale = 1f, bool withShadow = false)
        {
            if (withShadow)
            {
                Utility.drawWithShadow(b, image.Texture,
                    new Vector2((float)destination.X + (float)(image.SourceRect.Width / 2) * scale, (float)destination.Y + (float)(image.SourceRect.Height / 2) * scale),
                    image.SourceRect, color, 0f, new Vector2((float)(image.SourceRect.Width / 2), (float)(image.SourceRect.Height / 2)), scale, false, 0.86f + (float)destination.Y / 20000f, -1, -1, 0.35f);
            }
            else
            {
                b.Draw(image.Texture,
                    new Vector2((float)destination.X + (float)(image.SourceRect.Width / 2) * scale, (float)destination.Y + (float)(image.SourceRect.Height / 2) * scale), image.SourceRect,
                    color, 0f, new Vector2((float)(image.SourceRect.Width / 2), (float)(image.SourceRect.Height / 2)), scale, SpriteEffects.None, 0.86f + (float)destination.Y / 20000f);
            }
        }

        public static void DrawImageRotated(this SpriteBatch b, SdvImage image, Rectangle destination, float rotation, Color color, float scale = 1f, bool withShadow = false)
        {
            if (withShadow)
            {
                Utility.drawWithShadow(b, image.Texture,
                    new Vector2((float)destination.X + (float)(image.SourceRect.Width / 2) * scale, (float)destination.Y + (float)(image.SourceRect.Height / 2) * scale),
                    image.SourceRect, color, rotation, new Vector2((float)(image.SourceRect.Width / 2), (float)(image.SourceRect.Height / 2)), scale, false, 0.86f + (float)destination.Y / 20000f, -1, -1, 0.35f);
            }
            else
            {
                b.Draw(image.Texture,
                    new Vector2((float)destination.X + (float)(image.SourceRect.Width / 2) * scale, (float)destination.Y + (float)(image.SourceRect.Height / 2) * scale), image.SourceRect,
                    color, rotation, new Vector2((float)(image.SourceRect.Width / 2), (float)(image.SourceRect.Height / 2)), scale, SpriteEffects.None, 0.86f + (float)destination.Y / 20000f);
            }
        }

        #endregion

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

    }
}
