using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Utilities
{
    public static class LayoutHelper
    {
        public static Rectangle GetAlignedBounds(Rectangle containerBounds, Vector2 elementSize, ContentAlignment alignment)
        {
            var finalBounds = containerBounds;
            var newSize = Vector2.Min(new Vector2(containerBounds.Width, containerBounds.Height), elementSize);
            switch (alignment)
            {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomRight:
                    finalBounds = new Rectangle(finalBounds.X, finalBounds.Bottom - (int)newSize.Y, finalBounds.Width, (int)newSize.Y);
                    break;
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleRight:
                    finalBounds = new Rectangle(finalBounds.X, finalBounds.Center.Y - (int)(newSize.Y / 2f), finalBounds.Width, (int)newSize.Y);
                    break;
                default:
                    finalBounds = new Rectangle(finalBounds.X, finalBounds.Y, finalBounds.Width, (int)newSize.Y);
                    break;
            }
            switch (alignment)
            {
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    finalBounds.Width = (int)newSize.X;
                    break;
                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                    finalBounds.X = finalBounds.Center.X - (int)(newSize.X / 2f);
                    finalBounds.Width = (int)newSize.X;
                    break;
                default:
                    finalBounds.X = finalBounds.Right - (int)newSize.X;
                    finalBounds.Width = (int)newSize.X;
                    break;
            }
            return finalBounds;
        }
    }
}
