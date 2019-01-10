using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public SdvFont(SpriteFont sprite, bool bold = false, bool drawShadow = false, float scale = 1f)
        {
            Sprite = sprite;
            Bold = bold;
            DrawShadow = drawShadow;
            Scale = scale;
        }

        public static implicit operator SdvFont(SpriteFont font)
        {
            return new SdvFont(font);
        }

        public Vector2 MeasureString(string text)
        {
            var finalSize = Sprite.MeasureString(text) * Scale;
            if (!DrawShadow)
            {
                finalSize.Y -= 3;
            }
            return finalSize;
        }
    }
}
