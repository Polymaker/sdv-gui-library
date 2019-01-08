﻿using Microsoft.Xna.Framework;
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

        public bool Enabled { get; set; } = true;

        public bool Visible { get; set; } = true;

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

        public Rectangle ClientRectangle => GetClientRectangle();

        public Rectangle DisplayRectangle => GetDisplayRectangle();

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

        private SdvControl HoveringControl;

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            SdvControl targetControl = null;

            if (ActiveControl != null && Cursor.LeftButton == ButtonState.Pressed)
            {
                targetControl = ActiveControl;
            }
            else
            {
                targetControl = GetControlAtPosition(x, y);
            }

            var overControl = GetControlAtPosition(x, y);

            if (targetControl != null)
            {
                //if (HoveringControl != null && HoveringControl != targetControl)
                //{
                    
                //}
                HoveringControl = targetControl;
                var worldPoint = new Point(x, y);
                var localPt = targetControl.PointToLocal(worldPoint);
                ((ISdvCoreEvents)targetControl).OnMouseMove(new MouseEventArgs(localPt, worldPoint, MouseButtons.None));
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            var curMouse = Cursor;
            var scrolldata = new MouseEventArgs(Cursor, direction);

            foreach (var control in GetVisibleControls())
            {
                if (control.HandleScrollWheel(scrolldata))
                {
                    control.PerformScrollWheel(scrolldata.Delta);
                    break;
                }
                else if (control.ForwardScrollWheel(scrolldata))
                    break;
            }
        }

        public virtual bool CaptureMouseWheel(Point mousePosition)
        {
            return false;
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
