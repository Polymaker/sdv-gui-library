using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Polymaker.SdvUI.Controls
{
    public class SdvLabel : SdvControl
    {
        private bool _AutoSize;
        private SdvImage _Image;
        private TextImageRelation _TextImageRelation = TextImageRelation.ImageBeforeText;

        public bool AutoSize
        {
            get => _AutoSize;
            set
            {
                if (value != _AutoSize)
                {
                    _AutoSize = value;
                    OnAutoSizeChanged(EventArgs.Empty);
                }
            }
        }

        public ContentAlignment ImageAlign { get; set; } = ContentAlignment.MiddleCenter;

        public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleCenter;

        public TextImageRelation TextImageRelation
        {
            get => _TextImageRelation;
            set
            {
                if (value != _TextImageRelation)
                {
                    _TextImageRelation = value;
                    OnTextImageRelation(EventArgs.Empty);
                }
            }
        }

        public int TextImageSpacing { get; set; } = 8;

        public event EventHandler AutoSizeChanged;
        public event EventHandler TextImageRelationChanged;

        public SdvImage Image
        {
            get => _Image;
            set
            {
                if (value != _Image)
                {
                    _Image = value;
                }
            }
        }

        public bool DrawShadow { get; set; }

        public float TextureScale { get; set; } = 4f;

        public SdvLabel()
        {
            _AutoSize = true;
            Font = Game1.smallFont;
            BackColor = Color.Transparent;
            ForeColor = Color.Black;
        }

        protected virtual void OnAutoSizeChanged(EventArgs e)
        {
            AutoSizeChanged?.Invoke(this, e);
            AdjustSizeIfNeeded();
        }

        protected virtual void OnTextImageRelation(EventArgs e)
        {
            TextImageRelationChanged?.Invoke(this, e);
            AdjustSizeIfNeeded();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            AdjustSizeIfNeeded();
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            AdjustSizeIfNeeded();
        }

        protected virtual void AdjustSizeIfNeeded()
        {
            if (AutoSize)
            {
                Size = GetPreferredSize();
            }
        }

        public void RecalculateSize()
        {
            AdjustSizeIfNeeded();
        }

        protected override Point GetPreferredSize()
        {
            var prefSize = Padding.Size;
            var textSize = Vector2.Zero;
            var imgSize = Vector2.Zero;

            if (!string.IsNullOrEmpty(Text) && Font != null)
                textSize = Font.MeasureString(Text);

            if (Image != null)
                imgSize = Image.Size;

            var bothSet = textSize != Vector2.Zero && imgSize != Vector2.Zero;

            switch (TextImageRelation)
            {
                case TextImageRelation.Overlay:
                    prefSize += Vector2.Max(textSize, imgSize);
                    break;
                case TextImageRelation.ImageAboveText:
                case TextImageRelation.TextAboveImage:
                    prefSize += new Vector2(Math.Max(textSize.X, imgSize.X), 
                        textSize.Y + imgSize.Y + (bothSet ? TextImageSpacing : 0));
                    break;
                case TextImageRelation.ImageBeforeText:
                case TextImageRelation.TextBeforeImage:
                    prefSize += new Vector2(textSize.X + imgSize.X + (bothSet ? TextImageSpacing : 0), 
                        Math.Max(textSize.Y, imgSize.Y));
                    break;
            }

            return new Point((int)prefSize.X, (int)prefSize.Y);
        }

        protected override void OnDrawBackground(SpriteBatch b)
        {
            if (BackColor != Color.Transparent)
                b.Draw(Game1.staminaRect, GetDisplayRectangle(), BackColor);
        }

        protected override void OnDraw(SpriteBatch b)
        {
            base.OnDraw(b);
            var bounds = GetDisplayRectangle();

            var innerBounds = new Rectangle(bounds.X + Padding.Left, bounds.Y + Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical);

            var hasText = !string.IsNullOrEmpty(Text) && Font != null;
            var hasImage = Image != null;
            var hasBoth = hasText && hasImage;
            var textSize = hasText ? Font.MeasureString(Text) : Vector2.Zero;
            var textBounds = innerBounds;
            var imageBounds = innerBounds;

            if (hasBoth)
            {
                var ratioW = Image.Size.X / textSize.X;
                var ratioH = Image.Size.Y / textSize.Y;

                switch (TextImageRelation)
                {
                    case TextImageRelation.ImageAboveText:
                    case TextImageRelation.TextAboveImage:
                        imageBounds.Height = (int)((innerBounds.Height * ratioH) - (TextImageSpacing / 2f));
                        textBounds.Height = (int)((innerBounds.Height / ratioH) - (TextImageSpacing / 2f));
                        if (TextImageRelation == TextImageRelation.ImageAboveText)
                            textBounds.Y = innerBounds.Bottom - textBounds.Height;
                        else
                            imageBounds.Y = innerBounds.Bottom - imageBounds.Height;
                        break;
                    case TextImageRelation.ImageBeforeText:
                    case TextImageRelation.TextBeforeImage:
                        imageBounds.Width = (int)((innerBounds.Width * ratioW) - (TextImageSpacing / 2f));
                        textBounds.Width = (int)((innerBounds.Width / ratioW) - (TextImageSpacing / 2f));
                        if (TextImageRelation == TextImageRelation.ImageBeforeText)
                            textBounds.X = innerBounds.Right - textBounds.Width;
                        else
                            imageBounds.X = innerBounds.Right - imageBounds.Width;
                        break;
                }
            }

            if (hasImage)
            {
                imageBounds = GetAlignedBounds(imageBounds, Image.Size, TextAlign);
                b.DrawImage(Image, imageBounds);
            }

            if (hasText)
            {
                textBounds = GetAlignedBounds(textBounds, textSize, TextAlign);
                b.DrawString(Font, Text, new Vector2(textBounds.X, textBounds.Y), ForeColor);

                //Utility.drawTextWithShadow(b, Text,
                //    Font, new Vector2(bounds.Center.X, (bounds.Center.Y + 4)) - Game1.smallFont.MeasureString(Text) / 2f,
                //    ForeColor, 1f, -1f, -1, -1, 0f, 3);
            }
        }
    }
}
