using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MMS.BetterDirectoryListing {
	/// <summary>
	/// 
	/// </summary>
	public class ListEntry {
		#region Fields
		private string _extension;
		private FileSystemInfo _fileInfo;
		private string _filename;
		private bool _isDirectory;
		private string _path;
		private string _virtualPath;
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="virtualPath"></param>
		/// <param name="path"></param>
		/// <param name="isDirectory"></param>
		public ListEntry(string virtualPath, string path, bool isDirectory) {
			_virtualPath = virtualPath;
			_path = path;
			_isDirectory = isDirectory;
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public string Extension {
			get {
				if (_extension == null) { _extension = System.IO.Path.GetExtension(_path); }
				return _extension;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public FileSystemInfo FileInfo {
			get {
				if (_fileInfo == null) {
					_fileInfo = _isDirectory ? new DirectoryInfo(_path) : new FileInfo(_path) as FileSystemInfo;
				}
				return _fileInfo;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Filename {
			get {
				if (_filename == null) { _filename = System.IO.Path.GetFileName(_path); }
				return _filename;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsDirectory => _isDirectory;

		/// <summary>
		/// 
		/// </summary>
		public string Path => _path;

		/// <summary>
		/// 
		/// </summary>
		public string ViretualPath => _virtualPath;
		#endregion

		#region Methods
		public override string ToString() {
			var props = GetType().GetProperties().OrderBy(p => p.Name).ToList();
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			for (int i = 0; i < props.Count; i++) {
				var p = props[i];
				sb.Append(p.Name).Append("=");
				sb.Append(p.GetValue(this, null));
				if (i < props.Count - 1) sb.Append(", ");
			}
			sb.Append("}");
			return sb.ToString();
		}
		#endregion
	}
}
