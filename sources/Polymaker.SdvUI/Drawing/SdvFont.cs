using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class SdvFont
    {
        public SpriteFont Sprite { get; set; }
        public bool DrawShadow { get; set; }
        public bool Bold { get; set; }
        public float LineSpacing => Sprite.LineSpacing * Scale;
        public float Spacing => Sprite.Spacing * Scale;
        public float Scale { get; set; }
        public bool IsSpriteText { get; private set; }

        public SdvFont(SpriteFont sprite, bool bold = false, bool drawShadow = false, float scale = 1f)
        {
            Sprite = sprite;
            Bold = bold;
            DrawShadow = drawShadow;
            Scale = scale;
        }

        private SdvFont()
        {
            IsSpriteText = true;
            Scale = 1f;
        }

        public static implicit operator SdvFont(SpriteFont font)
        {
            return new SdvFont(font);
        }

        public Vector2 MeasureString(string text)
        {
            if (IsSpriteText)
            {
                return new Vector2(SpriteText.getWidthOfString(text), 
                    SpriteText.getHeightOfString(text));
            }
            var finalSize = Sprite.MeasureString(text) * Scale;
            if (!DrawShadow)
            {
                finalSize.Y -= 3;
            }
            return finalSize;
        }

        public static SdvFont OptionFont { get; } = new SdvFont();
        
        public static SdvFont DialogueFont => new SdvFont(Game1.dialogueFont);

        public static SdvFont SmallFont => new SdvFont(Game1.smallFont);

        public static SdvFont TinyFont => new SdvFont(Game1.tinyFont);
    }
}
