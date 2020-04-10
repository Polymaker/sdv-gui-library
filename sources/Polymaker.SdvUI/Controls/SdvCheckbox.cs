using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Polymaker.SdvUI.Drawing;
using StardewValley;

namespace Polymaker.SdvUI.Controls
{
    public class SdvCheckbox : SdvLabel
    {
        private bool _Checked;
        private SdvImage _CheckedImage;
        private SdvImage _UnCheckedImage;

        public bool Checked
        {
            get => _Checked;
            set
            {
                if(value != _Checked)
                {
                    _Checked = value;
                    OnCheckChanged(EventArgs.Empty);
                }
            }
        }

        public SdvImage CheckedImage
        {
            get => _CheckedImage;
            set
            {
                if (_CheckedImage != value)
                {
                    _CheckedImage = value;
                    AdjustSizeIfNeeded();
                }
            }
        }

        public SdvImage UnCheckedImage
        {
            get => _UnCheckedImage;
            set
            {
                if (_UnCheckedImage != value)
                {
                    _UnCheckedImage = value;
                    AdjustSizeIfNeeded();
                }
            }
        }

        public string CheckSound { get; set; }

        private int ImageWidth => (int)(CheckedImage ?? UnCheckedImage).Size.X;

        public event EventHandler CheckChanged;

        public SdvCheckbox()
        {
            _CheckedImage = new SdvImage(Game1.mouseCursors, StardewValley.Menus.OptionsCheckbox.sourceRectChecked, 4f);
            _UnCheckedImage = new SdvImage(Game1.mouseCursors, StardewValley.Menus.OptionsCheckbox.sourceRectUnchecked, 4f);
            CheckSound = "drumkit6";
        }

        public override Point GetPreferredSize()
        {
            var baseSize = base.GetPreferredSize();

            if (CheckedImage != null || UnCheckedImage != null)
            {
                baseSize.X += ImageWidth + ((Image != null || !string.IsNullOrEmpty(Text)) ? TextImageSpacing : 0);
                baseSize.Y = Math.Max(baseSize.Y, (int)(CheckedImage ?? UnCheckedImage).Size.Y + Padding.Vertical);
            }

            return baseSize;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Buttons == MouseButtons.Left && Enabled)
            {
                Checked = !Checked;
                if (!string.IsNullOrEmpty(CheckSound))
                    Game1.playSound(CheckSound);
            }
        }

        protected virtual void OnCheckChanged(EventArgs e)
        {
            CheckChanged?.Invoke(this, e);
        }

        public override Rectangle GetClientRectangle()
        {
            var baseRect = base.GetClientRectangle();
            baseRect.Width -= ImageWidth + TextImageSpacing;
            baseRect.X += ImageWidth + TextImageSpacing;
            return baseRect;
        }

        protected override void OnDraw(SdvGraphics g)
        {
            var checkBoxBounds = new Rectangle(Padding.Left, Padding.Top, ImageWidth, ImageWidth);
            g.DrawImage(Checked ? CheckedImage : UnCheckedImage, checkBoxBounds, Enabled ? Color.White : Color.Gray);
            if (!Enabled)
                g.FillRect(new Color(50, 50, 50, 120), checkBoxBounds);

            //g.Offset = new Point(g.Offset.X + ImageWidth, g.Offset.Y);
            base.OnDraw(g);
        }
    }
}
