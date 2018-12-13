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

        public event EventHandler<ControlsChangedEventArgs> CollectionChanged;

        public SdvControlCollection(ISdvContainer owner)
        {
            Owner = owner;
            Controls = new List<SdvControl>();
        }

        protected void OnCollectionChanged(ControlsChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        public void Add(SdvControl item)
        {
            if (ValidateCanAdd(item))
            {
                Controls.Add(item);
                item.SetParent(Owner, true);
                OnCollectionChanged(new ControlsChangedEventArgs(item, ControlsChangedEventArgs.Action.Add));
            }
        }

        public void AddRange(IEnumerable<SdvControl> items)
        {
            var validItems = items.Where(i => ValidateCanAdd(i)).ToList();
            if (validItems.Count > 0)
            {
                Controls.AddRange(validItems);
                validItems.ForEach(i => i.SetParent(Owner, true));
                OnCollectionChanged(new ControlsChangedEventArgs(validItems, ControlsChangedEventArgs.Action.Add));
            }
        }

        public void Insert(int index, SdvControl item)
        {
            if (ValidateCanAdd(item))
            {
                Controls.Insert(index, item);
                item.SetParent(Owner, true);
                OnCollectionChanged(new ControlsChangedEventArgs(item, ControlsChangedEventArgs.Action.Add));
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
                    item.SetParent(null, true);
                    OnCollectionChanged(new ControlsChangedEventArgs(item, ControlsChangedEventArgs.Action.Remove));
                }
            }
            return result;
        }

        public void RemoveAt(int index)
        {
            if (index <= Controls.Count)
            {
                var item = Controls[index];
                item.SetParent(null, true);
                Controls.RemoveAt(index);
                OnCollectionChanged(new ControlsChangedEventArgs(item, ControlsChangedEventArgs.Action.Remove));
            }
        }

        public void Clear()
        {
            if (Controls.Count > 0)
            {
                var controls = Controls.ToArray();
                Controls.ForEach(c =>
                {
                    c.SetParent(null, true);
                });
                Controls.Clear();
                OnCollectionChanged(new ControlsChangedEventArgs(controls, ControlsChangedEventArgs.Action.Remove));
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
