using System;
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
		private DirectoryBrowsingModuleConfigurationSection config;


		#region IHttpModule Members

		public void Dispose() { }

		public void Init(HttpApplication context) {
			context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
		}

		private void OnPreRequestHandlerExecute(object sender, EventArgs e) {
			HttpContext context = (sender as HttpApplication).Context;

			config = WebConfigurationManager.GetSection("directoryBrowsing", context.Request.Path) as DirectoryBrowsingModuleConfigurationSection;
			if (config == null) { throw new Exception("The <directoryBrowsing> configuration section is not registered on web.config."); }
			if (!config.Enabled) { throw new HttpException(403, null); }

			if (Directory.Exists(context.Request.PhysicalPath)) {
				string[] dirs = Directory.GetDirectories(context.Request.PhysicalPath);
				string[] files = Directory.GetFiles(context.Request.PhysicalPath);
				ListEntryCollection listing = new ListEntryCollection(dirs.Length+files.Length);
				foreach (var item in dirs) {
					listing.Add(new ListEntry(VirtualPathUtility.Combine(context.Request.Path + "/", Path.GetDirectoryName(item)), item, true));
				}
				foreach (var item in files) {
					listing.Add(new ListEntry(VirtualPathUtility.Combine(context.Request.Path + "/", Path.GetFileName(item)), item, false));
				}

				context.Items[DirectoryBrowsingContextKey] = listing;

				IHttpHandler template = new Template();
				template.ProcessRequest(context);
				context.Handler = null;
			}
		}

		#endregion



	}
}
