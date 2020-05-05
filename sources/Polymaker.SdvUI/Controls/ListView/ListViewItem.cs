using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class ListViewItem
    {
        private SdvFont _Font;
        private Color? _ForeColor;
        private Color? _BackColor;
        private bool _Selected;
        public string Text { get; set; }

        public SdvFont Font
        {
            get => _Font ?? ListView?.Font;
            set => _Font = value;
        }

        public Color ForeColor
        {
            get => _ForeColor ?? ListView?.ForeColor ?? Color.Black;
            set => _ForeColor = value;
        }

        public Color BackColor
        {
            get => _BackColor ?? ListView?.BackColor ?? Color.Transparent;
            set => _BackColor = value;
        }

        public SdvListView ListView { get; private set; }

        public int Index
        {
            get
            {
                if (ListView != null)
                    return ListView.Items.IndexOf(this);
                return -1;
            }
        }

        public bool Selected
        {
            get
            {
                if (ListView != null)
                {

                }
                return _Selected;
            }
            set
            {
                if (ListView != null)
                {

                }
                _Selected = value;
            }
        }

        public List<ListViewSubItem> SubItems { get; private set; }

        public class ListViewSubItem
        {
            public string Text { get; set; }

            public ListViewSubItem()
            {
            }

            public ListViewSubItem(string text)
            {
                Text = text;
            }
        }

        public ListViewItem()
        {
            SubItems = new List<ListViewSubItem>();
        }

        public ListViewItem(string text) : this()
        {
            Text = text;
        }

        internal void AssignListView(SdvListView listView)
        {
            ListView = listView;
        }
    }
}
