using Microsoft.Xna.Framework;
using Polymaker.SdvUI.Drawing;

namespace Polymaker.SdvUI.Controls
{
    public class ComboBoxItemDrawArgs
    {
        public SdvGraphics Graphics { get; }
        public object Item { get; }
        public int Index { get; }
        public string Value { get; set; }
        public bool IsSelected { get; }
        public bool IsOver { get; }
        public Rectangle ItemBounds { get; }

        public ComboBoxItemDrawArgs(SdvGraphics graphics, int index, object item, string value, bool isSelected, bool isOver, Rectangle itemBounds)
        {
            Graphics = graphics;
            Index = index;
            Item = item;
            Value = value;
            IsSelected = isSelected;
            IsOver = isOver;
            ItemBounds = itemBounds;
        }
    }
}
