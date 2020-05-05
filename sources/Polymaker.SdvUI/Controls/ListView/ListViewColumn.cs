namespace Polymaker.SdvUI.Controls
{
    public class ListViewColumn
    {
        private float _Width;
        private string _Text;

        public SdvListView ListView { get; private set; }

        public int Index
        {
            get
            {
                if (ListView != null)
                    return ListView.Columns.IndexOf(this);
                return -1;
            }
        }

        public float Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        public int CalculatedWidth { get; internal set; }

        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        public HorizontalAlignment HeaderAlignment { get; set; }

        public HorizontalAlignment TextAlignment { get; set; }



        internal void AssignListView(SdvListView listView)
        {
            ListView = listView;
        }
    }
}
