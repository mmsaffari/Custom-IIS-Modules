using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMS.BetterDirectoryListing {
	/// <summary>
	/// 
	/// </summary>
	public class ListEntryCollection : CollectionBase, IEnumerable<ListEntry> {

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		public ListEntryCollection() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		public ListEntryCollection(IEnumerable<ListEntry> source) {
			List.Clear();
			foreach (var item in source) {
				List.Add(item);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="capacity"></param>
		public ListEntryCollection(int capacity) : base(capacity) { }
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ListEntry this[int index] {
			get { return List[index] as ListEntry; }
			set { List[index] = value; }
		}

		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		public void Add(ListEntry entry) {
			List.Add(entry);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="entry"></param>
		public void Insert(int index, ListEntry entry) {
			List.Insert(index, entry);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		public void Remove(ListEntry entry) {
			List.Remove(entry);
		}

		#endregion

		#region IEnumerable Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IEnumerator<ListEntry> IEnumerable<ListEntry>.GetEnumerator() {
			foreach (ListEntry item in this.List) {
				yield return item;
			}
		}
		#endregion

	}
}
