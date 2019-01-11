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
    public class SdvForm : IClickableMenu, ISdvContainer
    {
        private Padding _Padding = Padding.Empty;
        private Point _ScrollOffset;
        private SdvWindowEventRouter EventManager;

        public SdvControl ActiveControl => EventManager.ActiveControl;
        public SdvControl HoveringControl => EventManager.HoveringControl;

        public SdvControlCollection Controls { get; }

        public bool Enabled { get; set; } = true;

        public bool Visible { get; set; } = true;

        public MouseState Cursor => EventManager.LastMouseState;

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

        public ISdvContainer Parent => null;

        public static readonly Padding GameMenuPadding = new Padding(16, 80, 16, 16);

        public const int GAME_MENU_BORDER = 16;

        public SdvForm()
        {
            Controls = new SdvControlCollection(this);
            EventManager = new SdvWindowEventRouter(this);
            _ScrollOffset = Point.Zero;
        }

        public SdvForm(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
        {
            Controls = new SdvControlCollection(this);
            EventManager = new SdvWindowEventRouter(this);
            _ScrollOffset = Point.Zero;
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            foreach (var control in Controls)
            {
                if(control.Visible)
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
    }
}
