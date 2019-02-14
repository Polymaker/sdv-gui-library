using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Polymaker.SdvUI.Drawing;
using StardewValley;

namespace Polymaker.SdvUI.Controls
{
    public class SdvComboBox : SdvControl
    {
        private static SdvImage DropDownArrowImage;
        private static SdvImage DropDownBgImage;

        private int _SelectedIndex;
        private IList _DataSource;
        private Rectangle DropDownArrowBounds;
        private int ItemHeight;
        private string _DisplayMember;
        private string _ValueMember;

        public IList DataSource
        {
            get => _DataSource;
            set
            {
                if (value != _DataSource)
                    SetDataSource(value);
            }
        }

        public string DisplayMember
        {
            get => _DisplayMember;
            set
            {
                if (_DisplayMember != value)
                    SetDiplayMember(value);
            }
        }

        public string ValueMember
        {
            get => _ValueMember;
            set
            {
                if (_ValueMember != value)
                    SetValueMember(value);
            }
        }

        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                if (value != _SelectedIndex && DataSource != null && value >= -1 && value < DataSource.Count)
                {
                    _SelectedIndex = value;
                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }

        public object SelectedItem
        {
            get
            {
                if (DataSource != null && SelectedIndex >= 0 && SelectedIndex < DataSource.Count)
                    return DataSource[SelectedIndex];
                return null;
            }
            set
            {
                if (value != SelectedItem)
                {
                    if (value == null)
                        SelectedIndex = -1;
                    else if (DataSource.Contains(value))
                        SelectedIndex = DataSource.IndexOf(value);
                }
            }
        }
        
        public object SelectedValue
        {
            get
            {
                if (SelectedItem != null)
                {
                    if (!string.IsNullOrEmpty(ValueMember) && valueProperty == null)
                        SetValueMember(ValueMember);

                    if (valueProperty != null)
                    {
                        return valueProperty.GetValue(SelectedItem);
                    }

                    return SelectedItem;
                }

                return null;
            }
            set
            {
                if (value != null && DataSource != null)
                {
                    if (!string.IsNullOrEmpty(ValueMember) && valueProperty == null)
                        SetValueMember(ValueMember);

                    if (valueProperty != null)
                    {
                        for(int i = 0; i < DataSource.Count; i++)
                        {
                            var itemValue = valueProperty.GetValue(DataSource[i]);
                            if (object.Equals(itemValue, value))
                            {
                                SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public bool DroppedDown { get; set; }

        public string NullValueText { get; set; }

        public event EventHandler SelectedIndexChanged;

        public SdvComboBox()
        {
            if (DropDownArrowImage == null)
            {
                DropDownArrowImage = new SdvImage(StardewValley.Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(437, 450, 10, 11));
                DropDownBgImage = new SdvImage(StardewValley.Game1.mouseCursors, new Rectangle(433, 451, 3, 3));
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            CalculateItemHeight();
        }

        private void CalculateItemHeight()
        {
            if (Font != null)
            {
                var textSize = Font.MeasureString("Qwerty");
                ItemHeight = (int)textSize.Y;
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            CalculateItemHeight();
        }

        protected override Point GetPreferredSize()
        {
            var size = base.GetPreferredSize();
            size.Y = ItemHeight + 4;
            return size;
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, ControlBounds specifiedBounds)
        {
            base.SetBoundsCore(x, y, width, height, specifiedBounds);
            float scale = Height / 11f;
            int arrowWidth = (int)(DropDownArrowImage.Size.X * scale);
            DropDownArrowBounds = new Rectangle(Width - arrowWidth, 0, arrowWidth, Height);
        }

        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }

        protected override void OnDraw(SdvGraphics g)
        {
            g.DrawTextureBox(DropDownBgImage, new Rectangle(0, 0, Width - DropDownArrowBounds.Width, Height), 4f);
            g.DrawImage(DropDownArrowImage, DropDownArrowBounds);

            if (SelectedItem != null || !string.IsNullOrEmpty(NullValueText))
            {
                var text = SelectedItem != null ? GetDisplayText(SelectedItem) : NullValueText;
                g.DrawString(text, Font, ForeColor, new Rectangle(8, 0, Width - DropDownArrowBounds.Width - 8, Height), ContentAlignment.MiddleLeft);
            }

            if (DroppedDown && DataSource != null)
            {
                int currentY = Height;
                int dropDownHeight = 8 + Math.Max(1, DataSource.Count) * ItemHeight;
                g.DrawTextureBox(DropDownBgImage, new Rectangle(0, currentY, Width, dropDownHeight), 4f);
                currentY += 4;

                for (int i = 0; i < DataSource.Count; i++)
                {
                    var itemBounds = new Rectangle(4, currentY, Width - 8, ItemHeight);
                    var textBounds = new Rectangle(8, currentY, Width - 8, ItemHeight);

                    if (i == SelectedIndex)
                    {
                        g.FillRect(Color.Wheat, itemBounds);
                    }

                    g.DrawString(GetDisplayText(DataSource[i]), Font, ForeColor, textBounds, ContentAlignment.MiddleLeft);
                    currentY += ItemHeight;
                }
            }
        }

        #region DataBinding Stuff

        private Type DataType;
        private PropertyDescriptor displayProperty;
        private PropertyDescriptor valueProperty;

        public string GetDisplayText(object item)
        {
            if (item != null)
            {
                if (!string.IsNullOrEmpty(DisplayMember) && displayProperty == null)
                    SetDiplayMember(DisplayMember);

                if (displayProperty != null)
                {
                    var value = displayProperty.GetValue(item);
                    return value?.ToString() ?? string.Empty;
                }

                return item.ToString();
            }

            return string.Empty;
        }

        private void SetDataSource(IList value)
        {
            _DataSource = value;
            _SelectedIndex = -1;
            displayProperty = null;
            valueProperty = null;
            DataType = null;

            if (value != null)
            {
                var listType = value.GetType();
                foreach (Type intType in listType.GetInterfaces())
                {
                    if (intType.IsGenericType)
                    {
                        var genericType = intType.GetGenericTypeDefinition();
                        if(genericType == typeof(IList<>) || genericType == typeof(IEnumerable<>) || genericType == typeof(ICollection<>))
                        {
                            DataType = intType.GetGenericArguments()[0];
                            break;
                        }
                    }
                }

                if (DataType == null && DataSource.Count > 0)
                {
                    DataType = DataSource[0].GetType();
                }
            }

            if (!string.IsNullOrEmpty(DisplayMember))
                SetDiplayMember(DisplayMember);

            if (!string.IsNullOrEmpty(ValueMember))
                SetDiplayMember(ValueMember);
        }

        private void SetDiplayMember(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && DataSource != null && (DataSource.Count > 0 || DataType != null))
            {
                if(DataType != null)
                    displayProperty = TypeDescriptor.GetProperties(DataType).Find(value, true);
                else if(DataSource.Count > 0)
                    displayProperty = TypeDescriptor.GetProperties(DataSource[0]).Find(value, true);

                if (displayProperty == null)
                    _DisplayMember = string.Empty;
            }
            else
                _DisplayMember = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
        }

        private void SetValueMember(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && DataSource != null && (DataSource.Count > 0 || DataType != null))
            {
                if (DataType != null)
                    valueProperty = TypeDescriptor.GetProperties(DataType).Find(value, true);
                else if (DataSource.Count > 0)
                    valueProperty = TypeDescriptor.GetProperties(DataSource[0]).Find(value, true);

                if (valueProperty == null)
                    _ValueMember = string.Empty;
            }
            else
                _ValueMember = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
        }

        #endregion

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (Enabled /*&& DropDownArrowBounds.Contains(CursorPosition)*/ && DataSource != null)
            {
                if (!DroppedDown)
                {
                    Game1.playSound("shwip");
                }
                DroppedDown = !DroppedDown;
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            DroppedDown = false;
        }

        public override bool HandleScrollWheel(MouseEventArgs data)
        {
            return Enabled && Focused && DataSource != null;
        }

        protected override void OnScrollWheel(int delta)
        {
            base.OnScrollWheel(delta);
            if (DataSource != null)
            {
                if (delta > 0 && SelectedIndex > 0)
                {
                    SelectedIndex--;
                }
                else if (delta < 0 && SelectedIndex < DataSource.Count - 1)
                {
                    SelectedIndex++;
                }
            }
        }

    }
}
