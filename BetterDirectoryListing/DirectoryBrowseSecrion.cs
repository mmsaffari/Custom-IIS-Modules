using System.Configuration;

namespace MMS.BetterDirectoryListing {
	class DirectoryBrowseSection: ConfigurationSection {
		public const bool DefaultEnabled = true;
		public const string ConfigurationSectionName = "directoryBrowsing";

		private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
		private static readonly ConfigurationProperty _propEnabled = new ConfigurationProperty("enabled", typeof(bool), true);

		static DirectoryBrowseSection() {
			_properties.Add(_propEnabled);
		}

		protected override ConfigurationPropertyCollection Properties => _properties;

		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled {
			get => (bool)base[_propEnabled];
			set => base[_propEnabled] = value;
		}
	}
}