using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class SdvControl : ISdvUIComponent
    {
        //public static SpriteFont DefaultFont => Game1.smallFont;

        private ISdvContainer _Parent;
        public bool Initialized { get; private set; }

        public ISdvContainer Parent
        {
            get => _Parent;
            set => SetParent(value);
        }

        internal void SetParent(ISdvContainer value, bool fromCollection = false)
        {
            if (value != _Parent)
            {
                if (!fromCollection)
                {
                    if (value != null && !value.Controls.ValidateCanAdd(this))
                    {
                        throw new InvalidOperationException();
                    }
                }

                if (_Parent != null && !(fromCollection && value == null))
                    _Parent.Controls.Remove(this);

                _Parent = value;

                if (!fromCollection && _Parent != null && !_Parent.Controls.Contains(this))
                    _Parent.Controls.Add(this);

                if (!Initialized && value != null && FinForm() != null)
                    Initialize();

                OnParentChanged(EventArgs.Empty);
            }
        }

        public SdvForm FinForm()
        {
            return FinForm(this);
        }

        private static SdvForm FinForm(SdvControl control)
        {
            if (control.Parent is SdvForm form)
                return form;
            else if (control.Parent is SdvControl parentControl)
                return FinForm(parentControl);
            return null;
        }

        public event EventHandler ParentChanged;

        protected virtual void OnParentChanged(EventArgs e)
        {
            ParentChanged?.Invoke(this, e);
        }

        private void Initialize()
        {
            if (Width == 0 || Height == 0)
                Size = GetPreferredSize();
            Initialized = true;
        }

        #region Text related members

        private SpriteFont _Font;
        private string _Text;

        public SpriteFont Font
        {
            get => _Font;
            set
            {
                if (value != _Font)
                {
                    _Font = value;
                    OnFontChanged(EventArgs.Empty);
                }
            }
        }

        public string Text
        {
            get => _Text;
            set
            {
                if (value != _Text)
                {
                    _Text = value;
                    OnTextChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler FontChanged;
        public event EventHandler TextChanged;

        protected virtual void OnFontChanged(EventArgs e)
        {
            FontChanged?.Invoke(this, e);
        }

        protected virtual void OnTextChanged(EventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }

        #endregion

        #region Size & Position related members

        private int _X;
        private int _Y;
        private int _Width;
        private int _Height;
        private Padding _Padding = Padding.Empty;
        private Rectangle? _CachedBounds;

        public int X
        {
            get => _X;
            set => SetBounds(value, 0, 0, 0, ControlBounds.X);
        }

        public int Y
        {
            get => _Y;
            set => SetBounds(0, value, 0, 0, ControlBounds.Y);
        }

        public Point Position
        {
            get => new Point(X, Y);
            set { X = value.X; Y = value.Y; }
        }

        public int Width
        {
            get => _Width;
            set => SetBounds(0, 0, value, 0, ControlBounds.Width);
        }

        public int Height
        {
            get => _Height;
            set => SetBounds(0, 0, 0, value, ControlBounds.Height);
        }

        public Point Size
        {
            get => new Point(Width, Height);
            set => SetBounds(0, 0, value.X, value.Y, ControlBounds.Size);
        }

        public virtual Padding Padding
        {
            get => _Padding;
            set
            {
                if (value != _Padding)
                {
                    _Padding = value;
                    OnPaddingChanged(EventArgs.Empty);
                }
            }
        }

        public Rectangle Bounds
        {
            get
            {
                if (_CachedBounds.HasValue)
                    return _CachedBounds.Value;
                _CachedBounds = new Rectangle(X, Y, Width, Height);
                return _CachedBounds.Value;
            }
            set => SetBounds(value.X, value.Y, value.Width, value.Height, ControlBounds.All);
        }

        public event EventHandler SizeChanged;

        public event EventHandler PaddingChanged;

        protected virtual void OnSizeChanged(EventArgs e)
        {
            SizeChanged?.Invoke(this, e);
        }

        protected virtual void OnPaddingChanged(EventArgs e)
        {
            PaddingChanged?.Invoke(this, e);
        }

        public virtual Rectangle GetDisplayRectangle()
        {
            if (Parent != null)
            {
                var parentBounds = Parent.GetDisplayRectangle();
                var contentOffset = Parent.GetClientRectangle();
                return new Rectangle(
                    parentBounds.X + contentOffset.X + X + Parent.ScrollOffset.X, 
                    parentBounds.Y + contentOffset.Y + Y + Parent.ScrollOffset.Y, 
                    Width, Height);
            }
            return Bounds;
        }

        public Point PointToDisplay(Point localPoint)
        {
            var db = GetDisplayRectangle();
            return new Point(db.X + localPoint.X, db.Y + localPoint.Y);
        }

        public Point PointToLocal(Point displayPoint)
        {
            var db = GetDisplayRectangle();
            return new Point(displayPoint.X - db.X, displayPoint.Y - db.Y);
        }

        #endregion

        private Color _BackColor = Color.White;
        private Color _ForeColor = Color.Black;

        public Color BackColor
        {
            get => _BackColor;
            set
            {
                if (value != _BackColor)
                {
                    _BackColor = value;
                    OnBackColorChanged(EventArgs.Empty);
                }
            }
        }

        public Color ForeColor
        {
            get => _ForeColor;
            set
            {
                if (value != _ForeColor)
                {
                    _ForeColor = value;
                    OnForeColorChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler BackColorChanged;
        public event EventHandler ForeColorChanged;

        protected virtual void OnBackColorChanged(EventArgs e)
        {
            BackColorChanged?.Invoke(this, e);
        }

        protected virtual void OnForeColorChanged(EventArgs e)
        {
            ForeColorChanged?.Invoke(this, e);
        }

        public bool Enabled { get; set; } = true;

        #region Size & Bounds Management

        public void SetBounds(int x, int y, int width, int height, ControlBounds specifiedBounds)
        {
            if (!specifiedBounds.HasFlag(ControlBounds.X))
                x = _X;
            if (!specifiedBounds.HasFlag(ControlBounds.Y))
                y = _Y;
            if (!specifiedBounds.HasFlag(ControlBounds.Width))
                width = _Width;
            if (!specifiedBounds.HasFlag(ControlBounds.Height))
                height = _Height;

            SetBoundsCore(x, y, width, height, specifiedBounds);
        }

        protected virtual void SetBoundsCore(int x, int y, int width, int height, ControlBounds specifiedBounds)
        {
            if (x != _X || y != _Y || width != _Width || height != _Height)
            {
                _X = x;
                _Y = y;
                _Width = width;
                _Height = height;
                _CachedBounds = null;
                OnSizeChanged(EventArgs.Empty);
            }
        }

        protected virtual Point GetPreferredSize()
        {
            return Point.Zero;
        }

        #endregion

        #region Mouse events

        public event EventHandler<SdvMouseEventArgs> MouseEnter;
        public event EventHandler<SdvMouseEventArgs> MouseLeave;
        public event EventHandler<SdvMouseEventArgs> MouseClick;
        public event EventHandler<SdvMouseEventArgs> MouseMove;

        public event EventHandler Click;

        protected virtual void OnMouseEnter(SdvMouseEventArgs sme)
        {
            MouseEnter?.Invoke(this, sme);
        }

        protected virtual void OnMouseLeave(SdvMouseEventArgs sme)
        {
            MouseLeave?.Invoke(this, sme);
        }

        protected virtual void OnMouseClick(SdvMouseEventArgs sme)
        {
            MouseClick?.Invoke(this, sme);
        }

        protected virtual void OnMouseMove(SdvMouseEventArgs sme)
        {
            MouseMove?.Invoke(this, sme);
        }

        protected internal virtual void OnLeftClick(Point pos)
        {
            Click?.Invoke(this, EventArgs.Empty);
        }
        

        #endregion

        protected virtual void OnDrawBackground(SpriteBatch b)
        {

        }

        protected virtual void OnDraw(SpriteBatch b)
        {

        }

        internal void PerformDraw(SpriteBatch b)
        {
            if(Width > 0 && Height > 0)
            {
                OnDrawBackground(b);
                OnDraw(b);
            }
        }

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
