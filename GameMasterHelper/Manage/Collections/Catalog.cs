using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameMasterHelper.Manage.Collections
{
    public class Catalog<TValue>
    {
		private ulong p_idStart;

		public ulong StartID
		{
			get 
			{
                _IfInsertInitBeginCheck();
                return p_idStart;
			}
			private set 
			{
				_IfInsertInitBeginCheck();
                p_idStart = value; 
			}
		}
		private void _IfInsertInitBeginCheck()
		{
            if (p_insertInitBegin)
                throw new Exception("End insert init!");
        }

		public Catalog(ulong idStart = 0ul) 
		{
            p_catalog = new SortedDictionary<ulong, TValue> ();
            StartID = idStart;
            p_id = StartID;
			p_insertInitBegin = false;
        }

		//string[] GetItemsNames()
		//{
		//	string[] names = new string[p_catalog.Count];
		//	var items = p_catalog.ToArray();
		//	for(int i =0; i < names.Length; ++i)
		//	{
		//		names[i] = ItemPrefix + items[i].Key.ToString() + ItemPostfix;
  //          }
		//	return names;
  //      }

		//private string p_itemPrefix;

		//public string ItemPrefix
		//{
		//	get { return p_itemPrefix; }
		//	set { p_itemPrefix = value; }
		//}

  //      private string p_itemPostfix;

  //      public string ItemPostfix
  //      {
  //          get { return p_itemPostfix; }
  //          set { p_itemPostfix = value; }
  //      }

        public void ClearItems()
		{
            _IfInsertInitBeginCheck();
            p_id = StartID;
            p_catalog.Clear ();
        }
		private void UpdateID()
		{
			ulong newId = p_idStart;
            foreach (var item in p_catalog)
            {
                if(item.Key != newId)
				{
					break;
				}
				++newId;
            }
            p_id = newId;
        }
		private bool p_insertInitBegin;
		public void BeginInsertInit()
		{
			if (p_insertInitBegin)
				throw new Exception("End previous insert init!");
			ClearItems();
			p_insertInitBegin = true;

        }
		public void Insert(ulong id, TValue value)
		{
            if (!p_insertInitBegin)
                throw new Exception("Begin insert init!");
			if(p_catalog.ContainsKey(id))
                throw new Exception("Value with this id already exist!");
			p_catalog.Add(id, value );
        }
        public void EndInsertInit()
        {
            if (!p_insertInitBegin)
                throw new Exception("Begin insert init!");
            UpdateID();
            p_insertInitBegin = false;
        }

        public ulong AddItem(TValue item)
		{
            _IfInsertInitBeginCheck();
            if (p_catalog.ContainsKey(p_id))
				throw new ArgumentException(item.GetType().Name + " item");
			p_catalog.Add(p_id, item);
			ulong id = p_id;
			UpdateID();
			return id;
		}

		public void RemoveItem(ulong key)
		{
            _IfInsertInitBeginCheck();
            p_catalog.Remove(key);
            UpdateID();
        }

		public TValue GetItem(ulong key)
		{
            _IfInsertInitBeginCheck();
            return p_catalog[key];

        }



		private ulong p_id;

		public ulong NextID
		{
			get 
			{
                _IfInsertInitBeginCheck();
                return p_id; 
			}
			private set 
			{
                _IfInsertInitBeginCheck();
                p_id = value;
			}
		}

		private SortedDictionary<ulong, TValue> p_catalog;

		public SortedDictionary<ulong, TValue> Items
		{
			get 
			{
                _IfInsertInitBeginCheck();
                var catalog_copy = p_catalog.Select(item =>
				new KeyValuePair<ulong, TValue>(item.Key, item.Value));

				return new SortedDictionary<ulong, TValue>( catalog_copy.ToDictionary(item => item.Key, item => item.Value));

            }
			private set 
			{
                _IfInsertInitBeginCheck();
                p_catalog = value;
			}
		}

	}
}
