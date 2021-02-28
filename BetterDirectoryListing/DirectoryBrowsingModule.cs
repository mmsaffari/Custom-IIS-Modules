using System;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Collections.Generic;
using System.Resources;
using System.Threading;

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
			context.BeginRequest += OnBeginRequest;
			context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
		}

		private void OnBeginRequest(object sender, EventArgs e) {
			Dictionary<string, Tuple<string, object>> dicStatics = new Dictionary<string, Tuple<string, object>> {
				{"/static/datatables/datatables.min.css", new Tuple<string, object>("text/css", "DataTables") },
				{"/static/fa/fontawesome-all.css", new Tuple<string, object>("text/css", "FontAwesonme") }
			};

			Dictionary<string, string> Mimes = new Dictionary<string, string> {

			};

			HttpContext context = (sender as HttpApplication).Context;
			HttpRequest Request = context.Request;
			HttpResponse Response = context.Response;
			if (Request.Path.ToLower().StartsWith("/static/")) {

				throw new Exception("TODO: make static requests caseINsensitive");

				var resNames = GetType().Assembly.GetManifestResourceNames().Select(n => n.ToLower()); ;
				string resName = string.Format("MMS.BetterDirectoryListing.Resources{0}", Request.Path.Replace("/", ".").Remove(0, "/static".Length)).ToLower();
				if (resNames.Contains(resName)) {
					var obj = GetResource(resName);
					if (obj != null) {
						//Response.ContentType = dicStatics[Request.Path].Item1;
						Response.Write(obj);
						Response.StatusCode = 200;
						Response.Flush();
						Response.End();
					}
				}
			}
			
			
		}

		private object GetResource(string name) {
			object result = null;
			using (var resStream= GetType().Assembly.GetManifestResourceStream(name))
			using (var ms = new MemoryStream()) {
				resStream.CopyTo(ms);
				byte[] ba = ms.ToArray();
				result = ba;
			}
			return result;
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
