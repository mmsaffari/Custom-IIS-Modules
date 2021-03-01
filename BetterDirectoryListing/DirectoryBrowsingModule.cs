using System;
<<<<<<< HEAD
using System.Linq;
=======
using System.Configuration;
>>>>>>> b1d242ebbd4d8f7b1a56a30abd0252e2bcd7599a
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace MMS.BetterDirectoryListing {
	/// <summary>
	/// 
	/// </summary>
	public class DirectoryBrowsingModule : IHttpModule {
<<<<<<< HEAD

		public const string DirectoryBrowsingContextKey = "MMS.BetterDirectoryListing";
		public string ModuleName => "DirectoryBrowsingModule";
		private string[] sensitiveItems = { "bin", "aspnet_client", "web.config" };
=======
		public const string DirectoryBrowsingContextKey = "MMS.BetterDirectoryListing";
>>>>>>> b1d242ebbd4d8f7b1a56a30abd0252e2bcd7599a

		#region IHttpModule Members

		public void Dispose() { }

		public void Init(HttpApplication context) {
			context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
		}

		private void OnPreRequestHandlerExecute(object sender, EventArgs e) {
			HttpContext context = (sender as HttpApplication).Context;

<<<<<<< HEAD
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
=======
			string configPath = "/web.config";
			Configuration config = WebConfigurationManager.OpenWebConfiguration(configPath);
			ConfigurationSection section = (ConfigurationSection)config.GetSection("system.webServer/directoryBrowse");
			context.Response.Clear();
			context.Response.Write(string.Format("[{0}]\n{1}", config, section.GetType()));
			context.Response.Flush();
>>>>>>> b1d242ebbd4d8f7b1a56a30abd0252e2bcd7599a
		}

		#endregion



	}
}
