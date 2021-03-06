﻿using Microsoft.Xna.Framework;
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
    public class SdvImage
    {
        public SdvImage(Texture2D texture, Rectangle sourceRect, float scale = 1f)
        {
            Texture = texture;
            SourceRect = sourceRect;
            Scale = scale;
        }

        public Texture2D Texture { get; set; }
        public Rectangle SourceRect { get; set; }

        public float Scale { get; set; }

        public Vector2 SourceSize => new Vector2(SourceRect.Width, SourceRect.Height);

        public Vector2 Size => SourceSize * Scale;
        
        public static SdvImage GetItemImage(StardewValley.Object item, float scale = 1f)
        {
            Rectangle imageSourceRect;
            Texture2D imageTexture;

            if (item.bigCraftable.Value)
            {
                imageSourceRect = StardewValley.Object.getSourceRectForBigCraftable(item.ParentSheetIndex);
                imageTexture = Game1.bigCraftableSpriteSheet;
            }
            else
            {
                imageSourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, item.ParentSheetIndex, 16, 16);
                imageTexture = Game1.objectSpriteSheet;
            }

            return new SdvImage(imageTexture, imageSourceRect, scale);
        }

        public static SdvImage GetStandardTileSheet(Texture2D tileSheet, int tilePosition, int width = -1, int height = -1)
        {
            Rectangle imageSourceRect = Game1.getSourceRectForStandardTileSheet(tileSheet, tilePosition, width, height);

            return new SdvImage(tileSheet, imageSourceRect, 1f);
        }
    }

    public static class SdvImages
    {
       
        public static SdvImage ButtonTexture => new SdvImage(Game1.mouseCursors, new Rectangle(432, 439, 9, 9));

        public static SdvImage UpArrow => new SdvImage(Game1.mouseCursors, new Rectangle(421, 459, 11, 12));

        public static SdvImage DownArrow => new SdvImage(Game1.mouseCursors, new Rectangle(421, 472, 11, 12));

        public static SdvImage LeftArrow => new SdvImage(Game1.mouseCursors, new Rectangle(352, 495, 12, 11));

        public static SdvImage RightArrow => new SdvImage(Game1.mouseCursors, new Rectangle(365, 495, 12, 11));

        public static SdvImage VScrollbarButton => new SdvImage(Game1.mouseCursors, new Rectangle(435, 463, 6, 10));

        public static SdvImage HScrollbarButton => new SdvImage(Game1.mouseCursors, new Rectangle(420, 441, 10, 6));

        public static SdvImage ScrollBarTrack => new SdvImage(Game1.mouseCursors, new Rectangle(403, 383, 6, 6));
    }
}
