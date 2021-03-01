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
using System.Text.RegularExpressions;

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
			HttpContext context = (sender as HttpApplication).Context;
			HttpRequest Request = context.Request;
			HttpResponse Response = context.Response;

			Regex rSlashes = new Regex("[/]{2,}");
			if (rSlashes.IsMatch(Request.Path)) { 
				Response.RedirectPermanent(rSlashes.Replace(Request.Path, "/"));
				Response.Flush();
				Response.End();
			}

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
			if (Request.Path == "/" && !config.AllowRoot) {
				Response.StatusCode = 403;
				Response.Flush();
				Response.End();
				throw new HttpException(403, null);
			}

			// Use Powershell to update this list.
			// Get-ChildItem -Recurse | Select-Object -Property Extension -Unique
			Dictionary<string, string> Mimes = new Dictionary<string, string> {
				{".css", "text/css"},
				{".eot", "application/vnd.ms-fontobject"},
				{".ico", "image/vnd.microsoft.icon"},
				{".js", "text/javascript"},
				{".map", "text/plain"},
				{".png", "image/png"},
				{".svg", "image/svg+xml"},
				{".ttf", "font/ttf"},
				{".woff", "font/woff"},
				{".woff2", "font/woff2"}
			};

			if (Request.Path.ToLower().StartsWith("/static/") || Request.Path.ToLower() == "/favicon.ico") {
				string pfn = Request.Path.Replace("/", ".");
				if (pfn.ToLower().StartsWith(".static")) pfn = pfn.Remove(0, ".static".Length);
				string resName = string.Format("MMS.BetterDirectoryListing.Resources{0}", pfn);
				var resNames = GetType().Assembly.GetManifestResourceNames();
				if (resNames.Any(name => name.ToLower() == resName.ToLower())) {
					var bytes = GetResource(resNames.Single(name => name.ToLower() == resName.ToLower()));
					if (bytes != null) {
						string filename = Request.Path.Split("/".ToCharArray()).Last();
						Response.BinaryWrite(bytes);
						Response.ContentType = Mimes[Path.GetExtension(filename)];
						Response.StatusCode = 200;
						Response.Flush();
						Response.End();
					}
				}
			}


		}

		private byte[] GetResource(string name) {
			byte[] result = null;
			using (var resStream = GetType().Assembly.GetManifestResourceStream(name))
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
			if (!config.Enabled) {
				context.Response.StatusCode = 403;
				context.Response.Flush();
				context.Response.End();
				throw new HttpException(403, null);
			}

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
				context.Items["isRootAllowed"] = config.AllowRoot;

				Template template = new Template();
				template.ProcessRequest(context);
				context.Handler = null;
			}
		}

		#endregion

	}
}
