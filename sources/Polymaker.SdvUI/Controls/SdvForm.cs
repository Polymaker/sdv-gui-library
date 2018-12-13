using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        public SdvControl ActiveControl { get; private set; }

        public SdvControlCollection Controls { get; }

        public MouseState Cursor => Mouse.GetState();

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

        public Rectangle GetClientRectangle()
        {
            return new Rectangle(Padding.Left, Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical);
        }

        public Rectangle GetDisplayRectangle()
        {
            return Bounds;
        }

        public Point ScrollOffset
        {
            get => _ScrollOffset;
            set
            {
                if (value != _ScrollOffset)
                {
                    _ScrollOffset = value;
                }
            }
        }

        public ISdvContainer Parent => null;

        public static readonly Padding GameMenuPadding = new Padding(16, 80, 16, 16);

        public const int GAME_MENU_BORDER = 16;

        public SdvForm()
        {
            Controls = new SdvControlCollection(this);
            _ScrollOffset = Point.Zero;
        }

        public SdvForm(int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(x, y, width, height, showUpperRightCloseButton)
        {
            Controls = new SdvControlCollection(this);
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

            //using (var clip = new GraphicClip(b, GetDisplayRectangle()))
            //{
            //    foreach (var control in Controls)
            //    {
            //        if(!(control is ISdvContainer))
            //            control.PerformDraw(b);
            //    }
            //}

            //foreach (var control in Controls)
            //{
            //    if (control is ISdvContainer)
            //        control.PerformDraw(b);
            //}
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

        #region Event Redirection

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            ActiveControl = GetControlAtPosition(x, y);

            if (ActiveControl != null)
            {
                var worldPoint = new Point(x, y);
                var localPt = ActiveControl.PointToLocal(worldPoint);
                ((ISdvCoreEvents)ActiveControl).OnMouseDown(new MouseEventArgs(localPt, worldPoint, MouseButtons.Left));
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            if (ActiveControl != null)
            {
                var worldPoint = new Point(x, y);
                var localPt = ActiveControl.PointToLocal(worldPoint);
                ((ISdvCoreEvents)ActiveControl).OnMouseUp(new MouseEventArgs(localPt, worldPoint, MouseButtons.Left));
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            var overControl = GetControlAtPosition(x, y);

            if (overControl != null)
            {
                var worldPoint = new Point(x, y);
                var localPt = overControl.PointToLocal(worldPoint);
                ((ISdvCoreEvents)overControl).OnMouseMove(new MouseEventArgs(localPt, worldPoint, MouseButtons.None));
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            var cursorPos = new Point(Cursor.X, Cursor.Y);

            foreach (var control in Controls)
            {
                var localPt = control.PointToLocal(cursorPos);

                if (control.Visible && control.CaptureMouseWheel(localPt.X, localPt.Y))
                    ((ISdvCoreEvents)control).OnScrollWheel(direction);
            }
        }

        public virtual bool CaptureMouseWheel(int x, int y)
        {
            return false;
        }

        #endregion

        public SdvControl GetControlAtPosition(int x, int y)
        {
            return GetControlAtPosition(this, x, y);
        }

        public SdvControl GetControlAtPosition(Point position)
        {
            return GetControlAtPosition(this, position.X, position.Y);
        }

        internal static SdvControl GetControlAtPosition(ISdvContainer container, int x, int y)
        {
            var controls = (container is ISdvContainerCore cc) ? cc.AllControls : container.Controls;

            foreach (var ctrl in controls)
            {
                if (ctrl.Visible && ctrl.GetDisplayRectangle().Contains(x, y))
                {
                    if (ctrl is ISdvContainer childContainer)
                        return GetControlAtPosition(childContainer, x, y);

                    return ctrl;
                }
            }

            if (container is SdvControl control)
                return control;

            return null;
        }

    }
}
