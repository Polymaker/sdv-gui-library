using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Polymaker.SdvUI.Utilities
{
    public class GraphicClip : IDisposable
    {
        private Rectangle OriginalClipRect;
        private Rectangle ClipRectangle;
        private SpriteBatch SB;
        private GraphicClip PreviousClip;
        private static Dictionary<SpriteBatch, GraphicClip> ExistingClips;
        public bool Invisible { get; private set; }

        static GraphicClip()
        {
            ExistingClips = new Dictionary<SpriteBatch, GraphicClip>();
        }

        public GraphicClip(SpriteBatch b, Rectangle clipRect)
        {
            if (ExistingClips.ContainsKey(b))
            {
                PreviousClip = ExistingClips[b];
                ExistingClips[b] = this;
            }
            else
            {
                PreviousClip = null;
                ExistingClips.Add(b, this);
            }
            OriginalClipRect = b.GraphicsDevice.ScissorRectangle;
            SB = b;

            var fixedRect = clipRect;
            fixedRect.X = Math.Max(clipRect.X, 0);
            fixedRect.Y = Math.Max(clipRect.Y, 0);
            fixedRect.Height = Math.Min(clipRect.Bottom, b.GraphicsDevice.Viewport.Height) - fixedRect.Y;
            fixedRect.Width = Math.Min(clipRect.Right, b.GraphicsDevice.Viewport.Width) - fixedRect.X;
            
            if(fixedRect.Width <=0 || fixedRect.Height <= 0)
            {
                Trace.WriteLine("Invalid Rect!!");
                throw  new InvalidOperationException("Invalid Clip Rect. " + fixedRect);
            }

            ClipRectangle = fixedRect;

            if (PreviousClip != null)
            {
                if (PreviousClip.ClipRectangle.Intersects(ClipRectangle))
                {
                    ClipRectangle = Rectangle.Intersect(ClipRectangle, PreviousClip.ClipRectangle);
                }
                else if (!PreviousClip.ClipRectangle.Contains(ClipRectangle))
                {
                    Trace.WriteLine("Warning! Child clip rectangle outside parent clip rect");
                    Invisible = true;
                }
            }

            ApplyClipping();
        }

        private void ApplyClipping()
        {
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState
            {
                ScissorTestEnable = true
            });
            SB.GraphicsDevice.ScissorRectangle = ClipRectangle;
        }

        public void Dispose()
        {
            if(PreviousClip != null)
            {
                ExistingClips[SB] = PreviousClip;
                PreviousClip.ApplyClipping();
            }
            else
            {
                ExistingClips.Remove(SB);
                SB.GraphicsDevice.ScissorRectangle = OriginalClipRect;
                SB.End();
                SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            }

            PreviousClip = null;
        }
    }
}
