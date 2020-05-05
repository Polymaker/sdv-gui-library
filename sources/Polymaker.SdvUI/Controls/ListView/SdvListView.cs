using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Polymaker.SdvUI.Drawing;
using Polymaker.SdvUI.Utilities;
using StardewValley;

namespace Polymaker.SdvUI.Controls
{
    public class SdvListView : SdvContainerControl
    {

        public ListViewColumnCollection Columns { get; private set; }

        public ListViewItemCollection Items { get; private set; }

        private Vector2 ContentSize;
        private Vector2 ViewSize;
        private int RowHeight;
        private int ColumnHeaderHeight;

        public SdvListView()
        {
            Columns = new ListViewColumnCollection(this);
            Columns.CollectionChanged += Columns_CollectionChanged;
            Items = new ListViewItemCollection(this);
            Items.CollectionChanged += Items_CollectionChanged;
        }

        

        protected override void OnInitialize()
        {
            base.OnInitialize();

            RowHeight = (int)Font.MeasureString("WasF").Y + 16;
            ColumnHeaderHeight = RowHeight;

            CalculateColumnBounds();
            InitializeScrollBars();

        }

        #region Scrolling

        public SdvScrollBar HScrollBar { get; private set; }

        public SdvScrollBar VScrollBar { get; private set; }

        public bool HScrollVisible => HScrollBar.Visible;

        public bool VScrollVisible => VScrollBar.Visible;

        private SdvScrollBar[] ScrollBars => new SdvScrollBar[] { HScrollBar, VScrollBar };

        private void InitializeScrollBars()
        {
            HScrollBar = new SdvScrollBar(Orientation.Horizontal, true);
            HScrollBar.SetParent(this, true);
            VScrollBar = new SdvScrollBar(Orientation.Vertical, true);
            VScrollBar.SetParent(this, true);

            HScrollBar.Visible = false;
            VScrollBar.Visible = false;

            //HScrollBar.Scroll += ScrollBars_Scroll;
            //VScrollBar.Scroll += ScrollBars_Scroll;

            UpdateScrollBars();
        }

        private void UpdateScrollBars()
        {
            HScrollBar.Visible = false;
            VScrollBar.Visible = false;

            VScrollBar.SmallChange = RowHeight;
            HScrollBar.SmallChange = 16;

            ContentSize = new Vector2
            {
                X = Columns.Sum(x => x.CalculatedWidth),
                Y = ColumnHeaderHeight + (RowHeight * Items.Count)
            };

            ViewSize = new Vector2(Width, Height);

            if (ContentSize.Y > Height)
            {
                VScrollBar.Visible = true;
                ViewSize.X -= VScrollBar.Width;
            }

            if (ContentSize.X > Width)
            {
                HScrollBar.Visible = true;
                ViewSize.Y -= HScrollBar.Height;

                if (!VScrollBar.Visible && ViewSize.Y < ContentSize.Y)
                {
                    VScrollBar.Visible = true;
                    ViewSize.X -= VScrollBar.Width;
                }
            }

            HScrollBar.MaxValue = (int)Math.Max(ContentSize.X - ViewSize.X, 1f);
            VScrollBar.MaxValue = (int)Math.Max(ContentSize.Y - ViewSize.Y, 1f);

            HScrollBar.SetBounds(0, Height - HScrollBar.Height, Width - (VScrollBar.Visible ? VScrollBar.Width : 0), 0, ControlBounds.Y | ControlBounds.Width);
            VScrollBar.SetBounds(Width - VScrollBar.Width, 0, 0, Height - (HScrollBar.Visible ? HScrollBar.Height : 0), ControlBounds.X | ControlBounds.Height);
        }

        protected override void OnScrollWheel(int delta)
        {
            base.OnScrollWheel(delta);

            if (Enabled && (VScrollVisible || HScrollVisible))
            {
                (VScrollVisible ? VScrollBar : HScrollBar).ProcessEvent(Events.SdvEvents.ScrollWheel, delta);
            }
        }

        public override bool HandleScrollWheel(MouseEventArgs data)
        {
            return Enabled && (VScrollVisible || HScrollVisible) && DisplayRectangle.Contains(data.Location);
        }

        public override IEnumerable<SdvControl> GetVisibleControls()
        {
            return base.GetVisibleControls().Concat(ScrollBars.Where(s => s.Visible));
        }

        public override bool Contains(SdvControl control)
        {
            return base.Contains(control) || ScrollBars.Contains(control);
        }

        //private void ScrollBars_Scroll(object sender, EventArgs e)
        //{

        //}

        #endregion

        private void Columns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!Initialized)
                return;
            CalculateColumnBounds();
            UpdateScrollBars();
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!Initialized)
                return;
            UpdateScrollBars();
        }

        #region Bounds Calculation

        private void CalculateColumnBounds()
        {
            int currentX = 0;
            int headerHeight = (int)Font.MeasureString("WasF").Y;

            foreach (var col in Columns)
            {
                float colWidth = col.Width;

                if (colWidth <= 0)
                    colWidth = Font.MeasureString(col.Text).X + 16;
                else if (colWidth <= 1f)
                    colWidth = Width * colWidth;

                col.CalculatedWidth = (int)colWidth;
                var headerBounds = new Rectangle(currentX, 0, (int)colWidth, headerHeight);

            }
        }

        public Rectangle GetColumnHeaderBounds(int index)
        {
            if (index >= Columns.Count)
                return new Rectangle();

            var baseBounds = new Rectangle(0, 0,
                Columns[index].CalculatedWidth,
                ColumnHeaderHeight);

            for (int i = 0; i < index; i++)
                baseBounds.X += Columns[i].CalculatedWidth;

            if (HScrollVisible)
                baseBounds.X -= HScrollBar.Value;

            return baseBounds;
        }

        public Rectangle GetItemBounds(int index)
        {
            var baseBounds = new Rectangle(0, 
                ColumnHeaderHeight + (RowHeight * index), 
                (int)ContentSize.X, 
                RowHeight);

            if (VScrollVisible)
                baseBounds.Y -= VScrollBar.Value;
            if (HScrollVisible)
                baseBounds.X -= HScrollBar.Value;
            return baseBounds;
        }

        public Rectangle GetSubItemBounds(int itemIndex, int subItemIndex)
        {
            var baseBounds = GetItemBounds(itemIndex);

            if (subItemIndex + 1 < Columns.Count)
            {
                baseBounds.Width = Columns[subItemIndex + 1].CalculatedWidth;
                for (int i = 0; i < subItemIndex + 1; i++)
                    baseBounds.X += Columns[i].CalculatedWidth;
            }
            return baseBounds;
        }

        #endregion

        protected override void OnDraw(SdvGraphics g)
        {
            DrawScrollBars(g);

            var headerClipRect = new Rectangle
            {
                X = ScreenBounds.X,
                Y = ScreenBounds.Y,
                Width = ScreenBounds.Width - (VScrollVisible ? VScrollBar.Width : 0),
                Height = ColumnHeaderHeight + 4
            };

            using (var clip = new GraphicClip(g.SB, headerClipRect))
            {
                if (clip.Invisible)
                    return;

                DrawColumnHeaders(g);
            }

            var viewClipRect = new Rectangle
            {
                X = ScreenBounds.X,
                Y = headerClipRect.Bottom,
                Width = ScreenBounds.Width - (VScrollVisible ? VScrollBar.Width : 0),
                Height = ScreenBounds.Height - headerClipRect.Height - (HScrollVisible ? HScrollBar.Height : 0)
            };

            using (var clip = new GraphicClip(g.SB, viewClipRect))
            {
                if (clip.Invisible)
                    return;

                DrawColumnSeparators(g);
                if (Items.Any())
                    DrawListViewItems(g);
            }
        }

        private void DrawScrollBars(SdvGraphics g)
        {
            foreach (var control in ScrollBars.Where(s => s.Visible))
                control.PerformDraw(g.SB);
        }

        private void DrawColumnHeaders(SdvGraphics g)
        {
            for (int i = 0; i < Columns.Count; i++)
            {
                var cellBounds = GetColumnHeaderBounds(i);
                var textBound = cellBounds;
                textBound.Inflate(-8, -8);
                var col = Columns[i];

                if (!string.IsNullOrEmpty(col.Text))
                    g.DrawString(col.Text, Font, ForeColor, textBound, 
                        GetContentAlignment(col.HeaderAlignment));

                DrawVerticalLine(g, cellBounds.Right, 0, RowHeight);
            }

            DrawHorizontalLine(g, 0, ColumnHeaderHeight, (int)ViewSize.X);
        }

        private void DrawColumnSeparators(SdvGraphics g)
        {
            for (int i = 0; i < Columns.Count; i++)
            {
                var cellBounds = GetColumnHeaderBounds(i);
                DrawVerticalLine(g, cellBounds.Right, 0, (int)ViewSize.Y);
            }
        }

        private void DrawListViewItems(SdvGraphics g)
        {
            int rowIndex = 0;

            foreach (var item in Items)
            {
                var rowBounds = GetItemBounds(rowIndex++);

                for (int i = 0; i < Columns.Count; i++)
                {
                    var cellBounds = GetSubItemBounds(rowIndex - 1, i - 1);
                    var textBound = cellBounds;
                    textBound.Inflate(-8, -8);

                    var textAlign = GetContentAlignment(Columns[i].TextAlignment);

                    if (i == 0)
                    {
                        if (!string.IsNullOrEmpty(item.Text))
                            g.DrawString(item.Text, item.Font, item.ForeColor, textBound, textAlign);
                    }
                    else if (i - 1 < item.SubItems.Count)
                    {
                        if (!string.IsNullOrEmpty(item.SubItems[i - 1].Text))
                            g.DrawString(item.SubItems[i - 1].Text, item.Font, item.ForeColor, textBound, textAlign);
                    }
                }

                DrawHorizontalLine(g, 0, rowBounds.Bottom, (int)ViewSize.X);
            }
        }

        private ContentAlignment GetContentAlignment(HorizontalAlignment alignment)
        {
            switch (alignment)
            {
                default:
                case HorizontalAlignment.Left:
                    return ContentAlignment.MiddleLeft;
                case HorizontalAlignment.Center:
                    return ContentAlignment.MiddleCenter;
                case HorizontalAlignment.Right:
                    return ContentAlignment.MiddleRight;
            }
        }

        private void DrawHorizontalLine(SdvGraphics g, int posX, int posY, int width)
        {
            var img = SdvImage.GetStandardTileSheet(Game1.menuTexture, 25);
            g.DrawImage(img, posX, posY - (int)(img.Size.Y / 2f), width, (int)img.Size.Y, Color.White);
        }

        private void DrawVerticalLine(SdvGraphics g, int posX, int posY, int height)
        {
            var img = SdvImage.GetStandardTileSheet(Game1.menuTexture, 26);
            g.DrawImage(img, posX - (int)(img.Size.X / 2f), posY,(int)img.Size.X,  height, Color.White);
        }

        #region Collection Classes

        public class ListViewColumnCollection : OwnerCollectionBase<SdvListView, ListViewColumn>
        {
            internal ListViewColumnCollection(SdvListView owner) : base(owner)
            {

            }

            protected override void SetChildOwner(ListViewColumn child)
            {
                child.AssignListView(Owner);
            }

            protected override void UnsetChildOwner(ListViewColumn child)
            {
                child.AssignListView(null);
            }
        }

        public class ListViewItemCollection : OwnerCollectionBase<SdvListView, ListViewItem>
        {
            internal ListViewItemCollection(SdvListView owner) : base(owner)
            {

            }

            public ListViewItem Add(string text)
            {
                var lvi = new ListViewItem(text);
                Add(lvi);
                return lvi;
            }

            public ListViewItem Add(string text, params string[] subItems)
            {
                var lvi = new ListViewItem(text);
                Add(lvi);
                for (int i = 0; i < subItems.Length; i++)
                    lvi.SubItems.Add(new ListViewItem.ListViewSubItem(subItems[i]));
                return lvi;
            }

            protected override void SetChildOwner(ListViewItem child)
            {
                child.AssignListView(Owner);
            }

            protected override void UnsetChildOwner(ListViewItem child)
            {
                child.AssignListView(null);
            }
        }

        #endregion
    }
}
