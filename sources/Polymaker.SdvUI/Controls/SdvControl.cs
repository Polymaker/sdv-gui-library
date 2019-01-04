using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Polymaker.SdvUI.Drawing;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class SdvControl : ISdvUIComponent, ISdvCoreEvents
    {
        //public static SpriteFont DefaultFont => Game1.smallFont;

        private ISdvContainer _Parent;
        public bool Initialized { get; private set; }
        public bool Active { get; internal set; }

        public ISdvContainer Parent
        {
            get => _Parent;
            set => SetParent(value);
        }

        public MouseState Cursor => Mouse.GetState();

        public Point CursorPosition
        {
            get
            {
                var mouse = Mouse.GetState();
                return PointToLocal(new Point(mouse.X, mouse.Y));
            }
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

        private SdvFont _Font;
        private string _Text;

        public SdvFont Font
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
        private const int CONTROL_BOUNDS = 0;
        private const int CLIENT_BOUNDS = 1;
        private const int PARENT_BOUNDS = 2;
        private const int DISPLAY_BOUNDS = 3;
        private Rectangle?[] CachedBounds = new Rectangle?[4];

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
                if (!CachedBounds[CONTROL_BOUNDS].HasValue)
                    CachedBounds[CONTROL_BOUNDS] = new Rectangle(X, Y, Width, Height);

                return CachedBounds[CONTROL_BOUNDS].Value;
            }
            set => SetBounds(value.X, value.Y, value.Width, value.Height, ControlBounds.All);
        }

        public Rectangle ClientRectangle
        {
            get
            {
                if (!CachedBounds[CLIENT_BOUNDS].HasValue)
                    CachedBounds[CLIENT_BOUNDS] = GetClientRectangle();

                return CachedBounds[CLIENT_BOUNDS].Value;
            }
        }

        public Rectangle DisplayRectangle
        {
            get
            {
                if (!CachedBounds[DISPLAY_BOUNDS].HasValue || HasParentBoundsChanged())
                    CachedBounds[DISPLAY_BOUNDS] = GetDisplayRectangle();

                return CachedBounds[DISPLAY_BOUNDS].Value;
            }
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

        private bool HasParentBoundsChanged()
        {
            if (CachedBounds[PARENT_BOUNDS].HasValue != (Parent != null))
                return true;

            if (Parent != null && CachedBounds[PARENT_BOUNDS].HasValue)
                return CachedBounds[PARENT_BOUNDS].Value == Parent.DisplayRectangle;

            return false;
        }

        public virtual Rectangle GetDisplayRectangle()
        {
            if (Parent != null)
            {
                var parentBounds = (CachedBounds[PARENT_BOUNDS] = Parent.DisplayRectangle).Value;
                var contentOffset = Parent.ClientRectangle;
                var scrollOffset = (Parent is IScrollableContainer sc ? sc.ScrollOffset : Point.Zero);
                return new Rectangle(
                    parentBounds.X + contentOffset.X + X - scrollOffset.X, 
                    parentBounds.Y + contentOffset.Y + Y - scrollOffset.Y, 
                    Width, Height);
            }
            else
            {
                CachedBounds[PARENT_BOUNDS] = null;
            }
            return Bounds;
        }

        public void InvalidateDisplayRectangle()
        {
            CachedBounds[DISPLAY_BOUNDS] = null;
        }

        public virtual Rectangle GetClientRectangle()
        {
            return new Rectangle(Padding.Left, Padding.Top, Width - Padding.Horizontal,  Height - Padding.Vertical);
        }

        public Point PointToDisplay(Point localPoint)
        {
            return new Point(DisplayRectangle.X + localPoint.X, DisplayRectangle.Y + localPoint.Y);
        }

        public Point PointToLocal(Point displayPoint)
        {
            return new Point(displayPoint.X - DisplayRectangle.X, displayPoint.Y - DisplayRectangle.Y);
        }

        #endregion

        private Color _BackColor = Color.Transparent;
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

        public bool Visible { get; set; } = true;

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
                CachedBounds[CONTROL_BOUNDS] = null;
                CachedBounds[CLIENT_BOUNDS] = null;
                CachedBounds[DISPLAY_BOUNDS] = null;
                OnSizeChanged(EventArgs.Empty);
            }
        }

        protected virtual Point GetPreferredSize()
        {
            return Point.Zero;
        }

        #endregion

        #region Mouse events

        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseClick;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<int> ScrollWheel;


        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }

        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            MouseClick?.Invoke(this, e);
        }

        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }

        protected virtual void OnScrollWheel(int delta)
        {
            ScrollWheel?.Invoke(this, delta);
        }

        public virtual bool CaptureMouseWheel(int x, int y)
        {
            return false;
        }

        #endregion

        #region Event redirection

        private MouseButtons PressedMouseButtons = MouseButtons.None;
        private Vector2 LastMousePress;

        void ISdvCoreEvents.OnMouseDown(MouseEventArgs e)
        {
            OnMouseDown(e);
            PressedMouseButtons |= e.Button;
            LastMousePress = new Vector2(e.X, e.Y);
        }

        void ISdvCoreEvents.OnMouseUp(MouseEventArgs e)
        {
            OnMouseUp(e);
            PressedMouseButtons ^= e.Button;
            var curPos = new Vector2(e.X, e.Y);
            if((curPos - LastMousePress).Length() < 3)
                OnMouseClick(new MouseEventArgs(e.Location, e.DisplayLocation, e.Button));
        }

        void ISdvCoreEvents.OnMouseMove(MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        void ISdvCoreEvents.OnScrollWheel(int delta)
        {
            OnScrollWheel(delta);
        }

        #endregion

        protected virtual void OnDrawBackground(SpriteBatch b)
        {
            if (BackColor != Color.Transparent)
                b.Draw(Game1.staminaRect, GetDisplayRectangle(), BackColor);
        }

        protected virtual void OnDraw(SpriteBatch b)
        {

        }

        protected virtual void OnDrawBackground2(SdvGraphics g)
        {
            if (BackColor != Color.Transparent)
                g.FillRect(BackColor, Bounds);
        }

        protected virtual void OnDraw2(SdvGraphics g)
        {

        }

        internal void PerformDraw(SpriteBatch b)
        {
            using (var g = new SdvGraphics(b, DisplayRectangle.Location))
                PerformDraw(g);

            //if (Width > 0 && Height > 0)
            //{
            //    OnDrawBackground(b);
            //    OnDraw(b);
            //}
        }
        internal void PerformDraw(SdvGraphics g)
        {
            if (Width > 0 && Height > 0)
            {
                OnDrawBackground2(g);
                OnDraw2(g);
            }
        }
    }
}
