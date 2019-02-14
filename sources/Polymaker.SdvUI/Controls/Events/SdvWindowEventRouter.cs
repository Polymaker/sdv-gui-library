using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls.Events
{
    public class SdvWindowEventRouter
    {
        private SdvControl _HoveringControl;
        private SdvControl _MouseCapturingControl;
        public SdvForm Owner { get; }

        public SdvControl MouseCapturingControl
        {
            get => _MouseCapturingControl;
            private set => SetMouseCapturingControl(value);
        }

        public SdvControl ActiveControl => Owner.ActiveControl;

        public SdvControl HoveringControl
        {
            get => _HoveringControl;
            private set
            {
                SetHoverControl(value);
            }
        }

        private MouseButtons HeldMouseButton = MouseButtons.None;
        private Vector2 PrevMousePos;
        public static InputState Input { get; private set; }
        public MouseState LastMouseState { get; private set; }

        static SdvWindowEventRouter()
        {
            Input = (InputState)typeof(Game1).GetField("input", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
        }

        public SdvWindowEventRouter(SdvForm owner)
        {
            Owner = owner;
            PrevMousePos = new Vector2(-1, -1);
        }

        #region Mouse events

        public void ReceiveLeftClick(int x, int y)
        {
            SdvControl target = null;

            if (ActiveControl?.IsCapturingMouse ?? false)
                target = ActiveControl;
            else
                target = Owner.GetControlAtPosition(x, y);

            if (!SetActiveControl(target))
                return;

            if (ActiveControl != null)
            {
                ActiveControl.ProcessEvent(SdvEvents.MouseDown, GetMouseEventArgs(target, x, y, MouseButtons.Left));
            }
        }

        public void LeftClickHeld(int x, int y)
        {
            if (ActiveControl != null && MouseCapturingControl == null)
            {
                if (ActiveControl.IsMouseButtonDown(MouseButtons.Left))
                {
                    MouseCapturingControl = ActiveControl;
                    HeldMouseButton = MouseButtons.Left;
                }
                else
                {
                    SetActiveControl(null);
                }
            }
        }

        public void ReleaseLeftClick(int x, int y)
        {
            SdvControl target = MouseCapturingControl ?? Owner.GetControlAtPosition(x, y);
            if (target != null)
                target.ProcessEvent(SdvEvents.MouseUp, GetMouseEventArgs(target, x, y, MouseButtons.Left));

            if (MouseCapturingControl != null && HeldMouseButton == MouseButtons.Left)
            {
                HeldMouseButton = MouseButtons.None;
                MouseCapturingControl = null;
            }
        }

        public void ReceiveRightClick(int x, int y)
        {
            SdvControl target = null;

            if (ActiveControl?.IsCapturingMouse ?? false)
                target = ActiveControl;
            else
                target = Owner.GetControlAtPosition(x, y);

            if (!SetActiveControl(target))
                return;

            if (ActiveControl != null)
            {
                ActiveControl.ProcessEvent(SdvEvents.MouseDown, GetMouseEventArgs(target, x, y, MouseButtons.Right));
            }
        }

        public void RightClickHeld(int x, int y)
        {
            if (ActiveControl != null && MouseCapturingControl == null)
            {
                if (ActiveControl.IsMouseButtonDown(MouseButtons.Right))
                {
                    MouseCapturingControl = ActiveControl;
                    HeldMouseButton = MouseButtons.Right;
                }
                else
                {
                    SetActiveControl(null);
                }
            }
        }

        public void ReleaseRightClick(int x, int y)
        {
            SdvControl target = MouseCapturingControl ?? Owner.GetControlAtPosition(x, y);
            if (target != null)
                target.ProcessEvent(SdvEvents.MouseUp, GetMouseEventArgs(target, x, y, MouseButtons.Right));

            if (MouseCapturingControl != null && HeldMouseButton == MouseButtons.Right)
            {
                HeldMouseButton = MouseButtons.None;
                MouseCapturingControl = null;
            }
        }

        public void PerformHoverAction(int x, int y)
        {
            UpdateMouseState(x, y);
            ValidateControlsState();

            var currentMousePos = new Vector2(x, y);

            if (currentMousePos != PrevMousePos)
            {
                if (MouseCapturingControl != null)
                {
                    HoveringControl = MouseCapturingControl;
                }
                else
                {
                    HoveringControl = Owner.GetControlAtPosition(x, y);
                }

                if (HoveringControl != null)
                {
                    HoveringControl.ProcessEvent(SdvEvents.MouseMove, GetMouseEventArgs(HoveringControl, x, y));
                }

                PrevMousePos = currentMousePos;
            }
        }

        public void ReceiveScrollWheelAction(int direction)
        {
            SdvControl target = MouseCapturingControl ?? HoveringControl;
            if (target != null)
            {
                var mousePos = GetMousePos();
                var mouseArg = GetMouseEventArgs(target, mousePos.X, mousePos.Y, MouseButtons.None, direction);

                if (target != MouseCapturingControl)
                {
                    while (target != null && !target.HandleScrollWheel(mouseArg))
                    {
                        if (target.Parent is SdvControl parentCtrl && parentCtrl != null)
                        {
                            target = parentCtrl;
                            mouseArg = mouseArg.ToLocal(parentCtrl);
                        }
                        else
                            break;
                    }
                }

                if (target != null)
                {
                    target.ProcessEvent(SdvEvents.ScrollWheel, direction);
                }
            }
        }

        private MouseEventArgs GetMouseEventArgs(ISdvUIComponent control, int x, int y, MouseButtons buttons = MouseButtons.None, int delta = 0)
        {
            var displayPoint = new Point(x, y);
            var localPoint = control.PointToLocal(displayPoint);
            return new MouseEventArgs(localPoint, displayPoint, buttons, delta);
        }

        private void UpdateMouseState(int x, int y)
        {
            var mouseState = Input.GetMouseState();
            LastMouseState = mouseState;

            if (mouseState.RightButton == ButtonState.Released && Game1.oldMouseState.RightButton == ButtonState.Pressed)
            {
                ReleaseRightClick(x, y);
            }
            else if (mouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton == ButtonState.Pressed)
            {
                RightClickHeld(x, y);
            }
        }

        private Point GetMousePos()
        {
            return new Point(
                (int)((float)Input.GetMouseState().X * (1f / Game1.options.zoomLevel)), 
                (int)((float)Input.GetMouseState().Y * (1f / Game1.options.zoomLevel)));
        }
        
        #endregion

        private void ValidateControlsState()
        {
            if (ActiveControl != null && (ActiveControl.Disposed || ActiveControl.FindForm() == null))
            {
                SetActiveControl(null);
            }

            if (MouseCapturingControl != null && (MouseCapturingControl.Disposed || MouseCapturingControl.FindForm() == null))
            {
                MouseCapturingControl = null;
                HeldMouseButton = MouseButtons.None;
            }
        }

        private bool SetActiveControl(SdvControl value)
        {
            if (value != null)
            {
                if (value.Parent != null)
                {
                    value.Parent.ActiveControl = value;
                    return Owner.ActiveControl == value;
                }
                return false;
            }

            return Owner.SetActiveControlInternal(value);
        }

        private void SetHoverControl(SdvControl value)
        {
            if (value != HoveringControl)
            {
                if (HoveringControl != null && !HoveringControl.Disposed)
                    HoveringControl.ProcessEvent(SdvEvents.MouseLeave, null);

                _HoveringControl = value;

                if (value != null)
                    value.ProcessEvent(SdvEvents.MouseEnter, null);
            }
        }

        private void SetMouseCapturingControl(SdvControl value)
        {
            if (value != MouseCapturingControl)
            {
                //if (MouseCapturingControl != null && !MouseCapturingControl.Disposed)
                //    MouseCapturingControl.IsCapturingMouse = false;

                _MouseCapturingControl = value;

                //if (value != null)
                //    value.IsCapturingMouse = true;
            }
        }
    }
}
