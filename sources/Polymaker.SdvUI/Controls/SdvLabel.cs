using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polymaker.SdvUI.Drawing;
using Polymaker.SdvUI.Utilities;
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
                    AdjustSizeIfNeeded();
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
        
        protected override void OnDraw(SdvGraphics g)
        {
            var hasText = !string.IsNullOrEmpty(Text) && Font != null;
            var hasImage = Image != null;
            var hasBoth = hasText && hasImage;
            var textSize = hasText ? Font.MeasureString(Text) : Vector2.Zero;
            var textBounds = ClientRectangle;
            var imageBounds = ClientRectangle;

            if (hasBoth)
            {
                var sumW = Image.Size.X + textSize.X;
                var sumH = Image.Size.Y + textSize.Y;

                switch (TextImageRelation)
                {
                    case TextImageRelation.ImageAboveText:
                    case TextImageRelation.TextAboveImage:
                        float availableHeight = ClientRectangle.Height - TextImageSpacing;
                        imageBounds.Height = (int)(availableHeight * (Image.Size.Y / sumH));
                        textBounds.Height = (int)(availableHeight - imageBounds.Height);

                        if (TextImageRelation == TextImageRelation.ImageAboveText)
                            textBounds.Y = ClientRectangle.Bottom - textBounds.Height;
                        else
                            imageBounds.Y = ClientRectangle.Bottom - imageBounds.Height;
                        break;
                    case TextImageRelation.ImageBeforeText:
                    case TextImageRelation.TextBeforeImage:
                        float availableWidth = ClientRectangle.Width - TextImageSpacing;
                        imageBounds.Width = (int)(availableWidth * (Image.Size.X / sumW));
                        textBounds.Width = (int)(availableWidth - imageBounds.Width);

                        if (TextImageRelation == TextImageRelation.ImageBeforeText)
                            textBounds.X = ClientRectangle.Right - textBounds.Width;
                        else
                            imageBounds.X = ClientRectangle.Right - imageBounds.Width;
                        break;
                }
            }

            if (hasImage)
            {
                imageBounds = LayoutHelper.GetAlignedBounds(imageBounds, Image.Size, ImageAlign);
                g.DrawImage(Image, imageBounds, Enabled ? Color.White : Color.Gray);
            }

            if (hasText)
            {
                textBounds = LayoutHelper.GetAlignedBounds(textBounds, textSize, TextAlign);
                g.DrawString(Text, Font, Enabled ? ForeColor : Color.Gray, textBounds);
            }
        }
    }
}
