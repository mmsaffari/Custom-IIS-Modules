using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MMS.BetterDirectoryListing {
	/// <summary>
	/// 
	/// </summary>
	public class DirectoryBrowsingModuleConfigurationSection : ConfigurationSection {
		/// <summary>
		/// 
		/// </summary>
		public const bool DefaultEnabled = true;
		/// <summary>
		/// 
		/// </summary>
		public const string ConfigurationSectionName = "directoryBrowsing";

		private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
		private static readonly ConfigurationProperty _propEnabled = new ConfigurationProperty("enabled", typeof(bool), true);
		private static readonly ConfigurationProperty _propHideSensitiveFiles = new ConfigurationProperty("hideSensitiveFiles", typeof(bool), true);
		private static readonly ConfigurationProperty _propAllowRoot = new ConfigurationProperty("allowRoot", typeof(bool), false);

		static DirectoryBrowsingModuleConfigurationSection() {
			_properties.Add(_propEnabled);
			_properties.Add(_propHideSensitiveFiles);
			_properties.Add(_propAllowRoot);
		}

		/// <summary>
		/// 
		/// </summary>
		protected override ConfigurationPropertyCollection Properties => _properties;

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled {
			get => (bool)base[_propEnabled];
			set => base[_propEnabled] = value;
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("hideSensitiveFiles", DefaultValue = true)]
		public bool HideSensitiveFiles {
			get => (bool)base[_propHideSensitiveFiles];
			set => base[_propHideSensitiveFiles] = value;
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("allowRoot", DefaultValue =false)]
		public bool AllowRoot {
			get => (bool)base[_propAllowRoot];
			set => base[_propAllowRoot] = value;
		}
	}
}
