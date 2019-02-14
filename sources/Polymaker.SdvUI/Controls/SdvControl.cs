using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Polymaker.SdvUI.Controls.Events;
using Polymaker.SdvUI.Drawing;
using StardewValley;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class SdvControl : ISdvUIComponent, IDisposable
    {

        #region Parent handling

        private ISdvContainer _Parent;
        private SdvForm _ParentForm;

        public ISdvContainer Parent
        {
            get => _Parent;
            set => SetParent(value);
        }

        public SdvForm ParentForm
        {
            get
            {
                //if (Parent is SdvForm form)
                //    return form;
                //else if (Parent is SdvControl parentControl)
                //    return parentControl.ParentForm;
                //return null;
                return _ParentForm;
            }
        }

        public event EventHandler ParentChanged;

        internal void SetParent(ISdvContainer value, bool fromCollection = false)
        {
            if (value != _Parent)
            {
                if (IsActiveControl)
                    Parent.ActiveControl = null;

                _ParentForm = (value as SdvControl)?.ParentForm;

                if (value != null && Disposed)
                    throw new ObjectDisposedException(GetType().Name);

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

                _ParentForm = FindForm();
                if (!Initialized && value != null && _ParentForm != null)
                    Initialize();

                OnParentChanged(EventArgs.Empty);
            }
        }

        public SdvForm FindForm()
        {
            return FindForm(this);
        }

        private static SdvForm FindForm(SdvControl control)
        {
            if (control.Parent is SdvForm form)
                return form;
            else if (control.Parent is SdvControl parentControl)
                return FindForm(parentControl);
            return null;
        }

        protected virtual void OnParentChanged(EventArgs e)
        {
            ParentChanged?.Invoke(this, e);
        }

        #endregion

        #region Cursor handling

        private Vector2[] MouseButtonsDownPos = new Vector2[4];
        private byte MouseButtonStates;

        public MouseState Cursor => ParentForm?.Cursor ?? Mouse.GetState();

        public Point CursorPosition
        {
            get
            {
                return PointToLocal(new Point(Cursor.X, Cursor.Y));
            }
        }

        public bool MouseOver => Visible && DisplayRectangle.Contains(CursorPosition);

        public bool IsCapturingMouse => MouseButtonStates > 0;

        public bool IsMouseButtonDown(MouseButtons button)
        {
            return (MouseButtonStates & (byte)button) == (byte)button;
            //return MouseButtonStates[(int)button];
        }

        #endregion

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

        public string TooltipText { get; set; }

        public string TooltipTitle { get; set; }

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
        private const int SCREEN_BOUNDS = 3;
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

        public Rectangle DisplayRectangle => new Rectangle(0, 0, Width, Height);

        public Rectangle ScreenBounds
        {
            get
            {
                if (!CachedBounds[SCREEN_BOUNDS].HasValue || HasParentBoundsChanged())
                    CachedBounds[SCREEN_BOUNDS] = GetScreenBounds();

                return CachedBounds[SCREEN_BOUNDS].Value;
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
                return CachedBounds[PARENT_BOUNDS].Value == Parent.ScreenBounds;

            return false;
        }

        public virtual Rectangle GetScreenBounds()
        {
            if (Parent != null)
            {
                var parentBounds = (CachedBounds[PARENT_BOUNDS] = Parent.ScreenBounds).Value;
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

        protected virtual void PropagateInvalidate()
        {
            if (this is SdvContainerControl sdvContainer)
            {
                foreach (var ctrl in sdvContainer.Controls)
                {
                    ctrl.Invalidate();
                }
            }
        }

        public void Invalidate()
        {
            CachedBounds[SCREEN_BOUNDS] = null;
            PropagateInvalidate();
        }

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
                CachedBounds[SCREEN_BOUNDS] = null;
                Invalidate();
                OnSizeChanged(EventArgs.Empty);
            }
        }

        protected virtual Point GetPreferredSize()
        {
            return Point.Zero;
        }

        public virtual Rectangle GetClientRectangle()
        {
            return new Rectangle(Padding.Left, Padding.Top, Width - Padding.Horizontal,  Height - Padding.Vertical);
        }

        public Point PointToDisplay(Point localPoint)
        {
            return new Point(ScreenBounds.X + localPoint.X, ScreenBounds.Y + localPoint.Y);
        }

        public Point PointToLocal(Point displayPoint)
        {
            return new Point(displayPoint.X - ScreenBounds.X, displayPoint.Y - ScreenBounds.Y);
        }


        #endregion

        #region Colors management

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

        #endregion

        #region Control state properties and methods

        private bool _Enabled = true;
        private bool _Visible = true;

        public bool Enabled
        {
            get
            {
                if (Parent != null && !Parent.Enabled)
                    return false;
                return _Enabled;
            }
            set
            {
                if (value != _Enabled)
                {
                    _Enabled = value;
                    OnEnabledChanged(EventArgs.Empty);
                }
            }
        }

        public bool Visible
        {
            get => _Visible;
            set
            {
                if (value != _Visible)
                {
                    _Visible = value;
                    if (!value && IsActiveControl)
                        Parent.ActiveControl = null;
                    OnVisibleChanged(EventArgs.Empty);
                }
            }
        }

        public bool Initialized { get; private set; }

        public bool Disposed { get; private set; }

        public event EventHandler EnabledChanged;
        public event EventHandler VisibleChanged;

        protected virtual void OnEnabledChanged(EventArgs e)
        {
            EnabledChanged?.Invoke(this, e);
        }

        protected virtual void OnVisibleChanged(EventArgs e)
        {
            VisibleChanged?.Invoke(this, e);
        }

        private void Initialize()
        {

            Initialized = true;
            OnInitialize();

            if (Width == 0 || Height == 0)
            {
                var minSize = GetPreferredSize();
                minSize.X = Math.Max(Width, minSize.X);
                minSize.Y = Math.Max(Height, minSize.Y);
                Size = minSize;
            }
        }

        protected virtual void OnInitialize()
        {

        }

        #endregion

        #region Activation/Focus management

        public bool IsActiveControl => Parent?.ActiveControl == this;
        public bool Focused { get; private set; }
        //public bool Focusable { get; set; } = true;

        //public event EventHandler Activated;
        //public event EventHandler Deactivated;
        public event EventHandler GotFocus;
        public event EventHandler LostFocus;
        public event CancelEventHandler Validating;

        //protected virtual void OnActivate(EventArgs e)
        //{
        //    Activated?.Invoke(this, e);
        //}

        //protected virtual void OnDeactivate(EventArgs e)
        //{
        //    Deactivated?.Invoke(this, e);
        //}

        protected virtual void OnGotFocus(EventArgs e)
        {
            GotFocus?.Invoke(this, e);
        }

        protected virtual void OnLostFocus(EventArgs e)
        {
            LostFocus?.Invoke(this, e);
        }

        internal bool TryRemoveFocus()
        {
            var args = new CancelEventArgs();
            OnValidating(args);
            return !args.Cancel;
        }

        protected virtual void OnValidating(CancelEventArgs args)
        {
            
        }

        public void Select()
        {
            if (Parent != null)
            {
                Parent.ActiveControl = this;
            }
        }

        #endregion

        #region Mouse events

        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseClick;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler MouseEnter;
        public event EventHandler MouseLeave;
        public event EventHandler Click;
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

        protected virtual void OnMouseEnter(EventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }

        protected virtual void OnMouseLeave(EventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }

        protected virtual void OnScrollWheel(int delta)
        {
            ScrollWheel?.Invoke(this, delta);
        }

        protected virtual void OnClick(EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public virtual bool HandleScrollWheel(MouseEventArgs data)
        {
            return false;
        }

        #endregion

        #region Drawing

        protected virtual void OnDrawBackground(SdvGraphics g)
        {
            if (BackColor != Color.Transparent)
                g.FillRect(BackColor, DisplayRectangle);
        }

        protected virtual void OnDraw(SdvGraphics g)
        {

        }

        internal void PerformDraw(SpriteBatch b)
        {
            if (Width > 0 && Height > 0)
            {
                using (var g = new SdvGraphics(b, ScreenBounds.Location))
                {
                    OnDrawBackground(g);
                    OnDraw(g);
                }
            }
        }

        #endregion

        internal void ProcessEvent(SdvEvents eventType, object data)
        {
            switch (eventType)
            {
                case SdvEvents.MouseDown:
                    {
                        var eventData = (MouseEventArgs)data;
                        MouseButtonsDownPos[(int)eventData.Buttons] = new Vector2(eventData.Location.X, eventData.Location.Y);
                        MouseButtonStates |= (byte)eventData.Buttons;
                        OnMouseDown(eventData);
                        break;
                    }
                case SdvEvents.MouseUp:
                    {
                        var eventData = (MouseEventArgs)data;
                        MouseButtonStates ^= (byte)eventData.Buttons;
                        OnMouseUp(eventData);

                        var curPos = new Vector2(eventData.Location.X, eventData.Location.Y);
                        if((curPos - MouseButtonsDownPos[(int)eventData.Buttons]).Length() < 4)
                        {
                            OnMouseClick((MouseEventArgs)data);
                            if (eventData.LeftButton)
                                OnClick(EventArgs.Empty);
                        }

                        break;
                    }
                case SdvEvents.MouseClick:
                    {
                        OnMouseClick((MouseEventArgs)data);
                        break;
                    }
                case SdvEvents.MouseMove:
                    {
                        OnMouseMove((MouseEventArgs)data);
                        break;
                    }
                case SdvEvents.ScrollWheel:
                    {
                        OnScrollWheel((int)data);
                        break;
                    }
                case SdvEvents.MouseEnter:
                    {
                        OnMouseEnter(data as EventArgs ?? EventArgs.Empty);
                        break;
                    }
                case SdvEvents.MouseLeave:
                    {
                        OnMouseLeave(data as EventArgs ?? EventArgs.Empty);
                        break;
                    }
                case SdvEvents.FocusChanged:
                    {
                        Focused = (bool)data;
                        if (Focused)
                            OnGotFocus(EventArgs.Empty);
                        else
                            OnLostFocus(EventArgs.Empty);
                        break;
                    }
            }
        }

        public SdvControl()
        {
            _Font = new SdvFont(Game1.smallFont, false, true);
        }

        ~SdvControl()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                if (Parent != null)
                {
                    if (Parent.Controls.Contains(this))
                        SetParent(null, false);
                    _Parent = null;
                }
                OnDispose();
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void OnDispose()
        {

        }
    }
}
