using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Polymaker.SdvUI.Controls.Events;
using Polymaker.SdvUI.Utilities;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class SdvForm : IClickableMenu, ISdvContainer, IDisposable
    {
        private Padding _Padding = Padding.Empty;
        private SdvWindowEventRouter EventManager;

        //public SdvControl ActiveControl => EventManager.ActiveControl;

        private SdvControl _ActiveControl;

        public SdvControl ActiveControl
        {
            get => _ActiveControl;
            set => SetActiveControl(value);
        }

        public SdvControl HoveringControl => EventManager.HoveringControl;

        public SdvControlCollection Controls { get; }

        public bool Enabled { get => true; set { } }

        public bool Visible
        {
            get => IsActiveMenu() || IsCurrentGameMenuPage();
            set { }
        }

        public bool Disposed { get; private set; }

        public MouseState Cursor => EventManager.LastMouseState;

        #region Size & Position handling

        public int X
        {
            get => xPositionOnScreen;
            set
            {
                if (value != xPositionOnScreen)
                    initialize(value, yPositionOnScreen, width, height, upperRightCloseButton != null);
            }
        }

        public int Y
        {
            get => yPositionOnScreen;
            set
            {
                if (value != yPositionOnScreen)
                    initialize(xPositionOnScreen, value, width, height, upperRightCloseButton != null);
            }
        }

        public Point Position
        {
            get => new Point(X, Y);
            set { X = value.X; Y = value.Y; }
        }

        public int Width
        {
            get => width;
            set
            {
                if (value != width)
                    initialize(xPositionOnScreen, yPositionOnScreen, value, height, upperRightCloseButton != null);
            }
        }

        public int Height
        {
            get => height;
            set
            {
                if (value != height)
                    initialize(xPositionOnScreen, yPositionOnScreen, width, value, upperRightCloseButton != null);
            }
        }

        public Point Size
        {
            get => new Point(Width, Height);
            set { Width = value.X; Height = value.Y; }
        }

        public virtual Padding Padding
        {
            get => _Padding;
            set
            {
                if (value != _Padding)
                {
                    _Padding = value;
                    //OnPaddingChanged(EventArgs.Empty);
                }
            }
        }

        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

        public Rectangle ClientRectangle => GetClientRectangle();

        public Rectangle DisplayRectangle => new Rectangle(0, 0, Width, Height);

        public Rectangle ScreenBounds => GetScreenBounds();

        public Rectangle GetClientRectangle()
        {
            return new Rectangle(Padding.Left, Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical);
        }

        public Rectangle GetScreenBounds()
        {
            return Bounds;
        }

        public Point PointToDisplay(Point localPoint)
        {
            var db = GetScreenBounds();
            return new Point(db.X + localPoint.X, db.Y + localPoint.Y);
        }

        public Point PointToLocal(Point displayPoint)
        {
            var db = GetScreenBounds();
            return new Point(displayPoint.X - db.X, displayPoint.Y - db.Y);
        }

        #endregion

        #region Events

        //public event EventArgs

        #endregion


        public ISdvContainer Parent => null;

        public static readonly Padding GameMenuPadding = new Padding(16, 80, 16, 16);

        public const int GAME_MENU_BORDER = 16;

        public SdvForm()
        {
            Controls = new SdvControlCollection(this);
            EventManager = new SdvWindowEventRouter(this);
        }

        public SdvForm(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
        {
            Controls = new SdvControlCollection(this);
            EventManager = new SdvWindowEventRouter(this);
        }

        ~SdvForm()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Controls.ClearAndDispose();
                OnDispose();
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void OnDispose()
        {

        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            foreach (var control in Controls)
            {
                if (control.Visible)
                    control.PerformDraw(b);
            }

            if (HoveringControl != null && !string.IsNullOrEmpty(HoveringControl.TooltipText))
            {
                if (!string.IsNullOrEmpty(HoveringControl.TooltipTitle))
                    drawToolTip(b, HoveringControl.TooltipText, HoveringControl.TooltipTitle, null);
                else
                    drawHoverText(b, HoveringControl.TooltipText, StardewValley.Game1.smallFont);
            }
        }

        #region Event Redirection

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            EventManager.ReceiveLeftClick(x, y);
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            EventManager.LeftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            EventManager.ReleaseLeftClick(x, y);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            base.receiveRightClick(x, y, playSound);
            EventManager.ReleaseRightClick(x, y);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            EventManager.PerformHoverAction(x, y);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            EventManager.ReceiveScrollWheelAction(direction);
        }

        #endregion

        #region ISdvContainer

        public SdvControl GetControlAtPosition(int x, int y)
        {
            return SdvContainerControl.GetControlAtPosition(this, x, y);
        }

        public SdvControl GetControlAtPosition(Point position)
        {
            return SdvContainerControl.GetControlAtPosition(this, position.X, position.Y);
        }

        public IEnumerable<SdvControl> GetVisibleControls()
        {
            return Controls.Where(c => c.Visible);
        }

        internal void SetActiveControl(SdvControl value)
        {
            if (value == _ActiveControl)
            {
                if (value == null || value.Focused)
                    return;
            }

            if (value != null && !Controls.Contains(value))
                throw new ArgumentException("The specified control is not owned by this form.");

            SetActiveControlInternal(value);
        }
        
        internal bool SetActiveControlInternal(SdvControl value)
        {
            if (_ActiveControl == value)
                return true;
            
            if (_ActiveControl != null)
            {
                if (!_ActiveControl.TryRemoveFocus())
                    return false;

                _ActiveControl.ProcessEvent(SdvEvents.FocusChanged, false);
                
                if (_ActiveControl.Parent is SdvContainerControl container)
                {
                    _ActiveControl = null;
                    container.SetActiveControlInternal(null);
                }
            }

            _ActiveControl = value;

            if (_ActiveControl != null)
            {
                _ActiveControl.ProcessEvent(SdvEvents.FocusChanged, false);
            }
            return true;
        }

        #endregion

        public void Show()
        {
            if (StardewValley.Game1.activeClickableMenu == null)
            {
                StardewValley.Game1.activeClickableMenu = this;
            }
        }

        public void Close()
        {
            if (IsActiveMenu() && readyToClose())
            {
                Dispose();
                StardewValley.Game1.activeClickableMenu = null;
            }
        }

        public bool IsActiveMenu()
        {
            if (StardewValley.Game1.activeClickableMenu == this)
                return true;

            return false;
        }

        public bool IsCurrentGameMenuPage()
        {
            if (StardewValley.Game1.activeClickableMenu is GameMenu gameMenu)
            {
                var currentPage = gameMenu.GetCurrentPage();
                return (currentPage == this);
            }
            return false;
        }

        
    }
}
