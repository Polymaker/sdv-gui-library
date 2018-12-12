using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polymaker.SdvUI.Controls
{
    public class SdvControlCollection : IList<SdvControl>
    {
        private List<SdvControl> Controls;

        public SdvControl this[int index] { get => Controls[index]; set => Controls[index] = value; }

        public int Count => Controls.Count;

        bool ICollection<SdvControl>.IsReadOnly => ((IList<SdvControl>)Controls).IsReadOnly;

        public ISdvContainer Owner { get; }

        //public event EventHandler CollectionChanged;
        public event EventHandler<ControlsChangedEventArgs> ControlAdded;
        public event EventHandler<ControlsChangedEventArgs> ControlRemoved;

        public SdvControlCollection(ISdvContainer owner)
        {
            Owner = owner;
            Controls = new List<SdvControl>();
        }

        protected void TriggerCollectionChanged(SdvControl control, ControlsChangedEventArgs.Action action)
        {
            //CollectionChanged?.Invoke(this, EventArgs.Empty);
            if (action == ControlsChangedEventArgs.Action.Add)
                ControlAdded?.Invoke(this, new ControlsChangedEventArgs(control, action));
            else if (action == ControlsChangedEventArgs.Action.Remove)
                ControlRemoved?.Invoke(this, new ControlsChangedEventArgs(control, action));
        }

        public void Add(SdvControl item)
        {
            if (ValidateCanAdd(item))
            {
                item.SetParent(Owner);
                Controls.Add(item);
                TriggerCollectionChanged(item, ControlsChangedEventArgs.Action.Add);
            }
        }

        public void Clear()
        {
            if (Controls.Count > 0)
            {
                Controls.ForEach(c =>
                {
                    c.SetParent(null);
                    TriggerCollectionChanged(c, ControlsChangedEventArgs.Action.Remove);
                });
                Controls.Clear();
            }
        }

        public bool Contains(SdvControl item)
        {
            return Controls.Contains(item);
        }

        public void CopyTo(SdvControl[] array, int arrayIndex)
        {
            Controls.CopyTo(array, arrayIndex);
        }

        public IEnumerator<SdvControl> GetEnumerator()
        {
            return Controls.GetEnumerator();
        }

        public int IndexOf(SdvControl item)
        {
            return Controls.IndexOf(item);
        }

        public void Insert(int index, SdvControl item)
        {
            if (ValidateCanAdd(item))
            {
                item.SetParent(Owner);
                Controls.Insert(index, item);
                TriggerCollectionChanged(item, ControlsChangedEventArgs.Action.Add);
            }
        }

        public bool Remove(SdvControl item)
        {
            bool result = false;
            try
            {
                result = Controls.Remove(item);
            }
            finally
            {
                if (result)
                {
                    item.SetParent(null);
                    TriggerCollectionChanged(item, ControlsChangedEventArgs.Action.Remove);
                }
            }
            return result;
        }

        public void RemoveAt(int index)
        {
            if (index <= Controls.Count)
            {
                var item = Controls[index];
                item.SetParent(null);
                Controls.RemoveAt(index);
                TriggerCollectionChanged(item, ControlsChangedEventArgs.Action.Remove);
            }
        }

        public bool ValidateCanAdd(SdvControl control)
        {
            if (Contains(control))
                return false;

            if (control == Owner)
                return false;

            if (CheckCircularReference(control))
                return false;

            return true;
        }

        private bool CheckCircularReference(SdvControl control)
        {
            if (control is ISdvContainer container)
            {
                foreach (var child in container.Controls)
                {
                    if (child == Owner)
                        return true;
                    return CheckCircularReference(child);
                }
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Controls.GetEnumerator();
        }
    }
}
