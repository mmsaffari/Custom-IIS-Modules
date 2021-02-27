using System;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace MMS.BetterDirectoryListing {
	/// <summary>
	/// 
	/// </summary>
	public class DirectoryBrowsingModule : IHttpModule {

		public const string DirectoryBrowsingContextKey = "MMS.BetterDirectoryListing";
		public string ModuleName => "DirectoryBrowsingModule";
		private string[] sensitiveItems = { "bin", "aspnet_client", "web.config" };

		#region IHttpModule Members

		public void Dispose() { }

		public void Init(HttpApplication context) {
			context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
		}

		private void OnPreRequestHandlerExecute(object sender, EventArgs e) {
			HttpContext context = (sender as HttpApplication).Context;

			var config = WebConfigurationManager.GetSection(
				DirectoryBrowsingModuleConfigurationSection.ConfigurationSectionName,
				context.Request.Path
			) as DirectoryBrowsingModuleConfigurationSection;
			if (config == null) {
				throw new Exception(string.Format(
					"The <{0} /> configuration section is not registered on web.config.",
					DirectoryBrowsingModuleConfigurationSection.ConfigurationSectionName
				));
			}
			if (!config.Enabled) { throw new HttpException(403, null); }

			if (Directory.Exists(context.Request.PhysicalPath)) {
				string[] dirs = Directory.GetDirectories(context.Request.PhysicalPath);
				string[] files = Directory.GetFiles(context.Request.PhysicalPath);
				ListEntryCollection listing = new ListEntryCollection();
				foreach (var item in dirs) {
					if (config.HideSensitiveFiles && sensitiveItems.Contains(Path.GetFileName(item).ToLower())) continue;
					listing.Add(new ListEntry(VirtualPathUtility.Combine(context.Request.Path + "/", Path.GetFileName(item)), item, true));
				}
				foreach (var item in files) {
					if (config.HideSensitiveFiles && sensitiveItems.Contains(Path.GetFileName(item).ToLower())) continue;
					listing.Add(new ListEntry(VirtualPathUtility.Combine(context.Request.Path + "/", Path.GetFileName(item)), item, false));
				}

				context.Items[DirectoryBrowsingContextKey] = listing;

				Template template = new Template();
				template.ProcessRequest(context);
				context.Handler = null;
			}
		}

		#endregion



	}
}
