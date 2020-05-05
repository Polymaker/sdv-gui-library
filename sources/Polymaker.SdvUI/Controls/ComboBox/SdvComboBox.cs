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
        private int MinimumWidth;
        private string _DisplayMember;
        private string _ValueMember;

        public ComboBoxStyle DropDownStyle { get; set; }

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
            DropDownStyle = ComboBoxStyle.Stardew;
            

            InitializeResources();

            Padding = new Padding(6, 8, 4, 2);
            Height = 44;
        }

        private static void InitializeResources()
        {
            if (DropDownArrowImage == null && Game1.mouseCursors != null)
            {
                DropDownArrowImage = new SdvImage(Game1.mouseCursors, new Rectangle(437, 450, 10, 11));
                DropDownBgImage = new SdvImage(Game1.mouseCursors, new Rectangle(433, 451, 3, 3));
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
                var textSize = Font.MeasureString("Qwerty123");
                ItemHeight = (int)textSize.Y + Padding.Vertical;

                textSize = Font.MeasureString("W");
                MinimumWidth = (int)textSize.X + Padding.Horizontal;

                if (DropDownArrowImage != null)
                {
                    float scale = ItemHeight / 11f;
                    int arrowWidth = (int)(DropDownArrowImage.Size.X * scale);
                    MinimumWidth += arrowWidth;
                }
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            CalculateItemHeight();
        }

        public override Point GetPreferredSize()
        {
            var size = base.GetPreferredSize();
            size.Y = ItemHeight;
            size.X = MinimumWidth;
            return size;
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, ControlBounds specifiedBounds)
        {
            base.SetBoundsCore(x, y, width, height, specifiedBounds);

            if (Height > 0 && Width > 0)
            {
                float scale = Height / 11f;
                int arrowWidth = (int)(DropDownArrowImage.Size.X * scale);
                DropDownArrowBounds = new Rectangle(Width - arrowWidth, 0, arrowWidth, Height);
            }
            
        }

        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }

        #region Drawing

        protected override void OnDraw(SdvGraphics g)
        {
            g.DrawTextureBox(DropDownBgImage, new Rectangle(0, 0, Width - DropDownArrowBounds.Width, Height), 4f);
            g.DrawImage(DropDownArrowImage, DropDownArrowBounds);

            if (SelectedItem != null || !string.IsNullOrEmpty(NullValueText))
            {
                var text = SelectedItem != null ? GetDisplayText(SelectedItem) : NullValueText;

                g.DrawString(text, Font, ForeColor, 
                    new Rectangle(Padding.Left, Padding.Top, 
                    Width - DropDownArrowBounds.Width - Padding.Horizontal, 
                    Height - Padding.Vertical), 
                    ContentAlignment.MiddleLeft);
            }

            if (DroppedDown && DataSource != null)
            {
                int currentY = 0;
                int dropDownHeight = Math.Max(1, DataSource.Count) * ItemHeight;
                int dropDownWidth = Width;

                if (DropDownStyle == ComboBoxStyle.Windows)
                    currentY = Height;
                else
                    dropDownWidth -= DropDownArrowBounds.Width;

                g.DrawTextureBox(DropDownBgImage, new Rectangle(0, currentY, dropDownWidth, dropDownHeight), 4f);

                for (int i = 0; i < DataSource.Count; i++)
                {
                    var itemBounds = new Rectangle(0, currentY, dropDownWidth, ItemHeight);
                    var args = new ComboBoxItemDrawArgs(g, i, DataSource[i], GetDisplayText(DataSource[i]), i == SelectedIndex, false, itemBounds);
                    DrawItem(args);
                    currentY += ItemHeight;
                }

            }
        }

        protected virtual void DrawItem(ComboBoxItemDrawArgs dia)
        {
            if (dia.IsSelected)
                dia.Graphics.FillRect(Color.Wheat, dia.ItemBounds);

            var textBounds = dia.ItemBounds;
            textBounds.X += Padding.Left;
            textBounds.Y += Padding.Top;
            textBounds.Width -= Padding.Horizontal;
            textBounds.Height -= Padding.Vertical;
            dia.Graphics.DrawString(dia.Value, Font, ForeColor, textBounds, ContentAlignment.MiddleLeft);
        }

        #endregion

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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (DropDownStyle == ComboBoxStyle.Stardew && !DroppedDown && 
                Enabled && e.LeftButton && DataSource != null)
            {
                DroppedDown = true;
                Game1.playSound("shwip");
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (DropDownStyle == ComboBoxStyle.Stardew && DroppedDown &&
                Enabled && e.LeftButton && DataSource != null)
            {
                DroppedDown = false;
                Game1.playSound("drumkit6");
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (DropDownStyle == ComboBoxStyle.Windows && Enabled  && DataSource != null)
            {
                if (!DroppedDown)
                    Game1.playSound("shwip");
                else
                    Game1.playSound("drumkit6");
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
