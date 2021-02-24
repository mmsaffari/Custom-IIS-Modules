using System;
using System.Configuration;
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

		#region IHttpModule Members

		public void Dispose() { }

		public void Init(HttpApplication context) {
			context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
		}

		private void OnPreRequestHandlerExecute(object sender, EventArgs e) {
			HttpContext context = (sender as HttpApplication).Context;

			string configPath = "/web.config";
			Configuration config = WebConfigurationManager.OpenWebConfiguration(configPath);
			ConfigurationSection section = (ConfigurationSection)config.GetSection("system.webServer/directoryBrowse");
			context.Response.Clear();
			context.Response.Write(string.Format("[{0}]\n{1}", config, section.GetType()));
			context.Response.Flush();
		}

		#endregion



	}
}
