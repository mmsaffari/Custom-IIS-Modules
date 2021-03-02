using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace MMS.BetterDirectoryListing {
	class Template : Page {

		public ListEntryCollection ListEntries { get; set; }
		public bool IsRootAllowed { get; set; }

		HyperLink hlNavigateUp = new HyperLink();
		Repeater rptMain = new Repeater();

		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			AppRelativeVirtualPath = VirtualPathUtility.AppendTrailingSlash(Context.Request.Path);
			Controls.Clear();
			var html = new HtmlHtml();
			var head = new HtmlHead();

			#region HTML Head
			head.Title = string.Format("Directory contents of {0}", Context.Request.Path);
			var linkDataTablesCSS = new HtmlLink() { Href = "/static/datatables/css/jquery.dataTables.min.css" };
			linkDataTablesCSS.Attributes.Add("rel", "stylesheet");
			linkDataTablesCSS.Attributes.Add("type", "text/css");
			head.Controls.Add(linkDataTablesCSS);
			var linkFontAwesomeCSS = new HtmlLink() { Href = "/static/font_awesome/css/all.min.css" };
			linkFontAwesomeCSS.Attributes.Add("rel", "stylesheet");
			linkFontAwesomeCSS.Attributes.Add("type", "text/css");
			head.Controls.Add(linkFontAwesomeCSS);
			var styleInline = new HtmlStyle();
			styleInline.InnerHtml = Properties.Resources.inlineStyleInHead;
			head.Controls.Add(styleInline);
			#endregion

			var body = new HtmlBody();

			#region HTML Body Content
			var topDiv = new Panel();
			topDiv.Style.Add("height", "80px");
			topDiv.Controls.Add(new HtmlH2 { InnerText = GetPath() });
			hlNavigateUp.Text = Properties.Resources.toParentDirectory;
			topDiv.Controls.Add(hlNavigateUp);
			body.Controls.Add(topDiv);
			HtmlForm form = new HtmlForm();

			var tbRptHead = new TemplateBuilder();
			tbRptHead.AppendLiteralString("<table id=\"DirectoryListing\" style=\"width: 100%\"><thead><tr><th>Name</th><th>Created</th><th>Last Modified</th><th>Size</th></tr></thead><tbody>");

			rptMain.ItemDataBound += RptMain_ItemDataBound;

			var tbRptFoot = new TemplateBuilder();
			tbRptFoot.AppendLiteralString("</tbody></table>");

			rptMain.HeaderTemplate = tbRptHead;
			rptMain.FooterTemplate = tbRptFoot;

			form.Controls.Add(rptMain);
			body.Controls.Add(form);
			#endregion

			#region HTML Body Foot Scripts
			body.Controls.Add(new HtmlScript { Src = "/static/jquery/jquery.js" });
			body.Controls.Add(new HtmlScript { Src = "/static/datatables/js/jquery.dataTables.js" });
			body.Controls.Add(new HtmlScript { InnerHtml = Properties.Resources.inlineScriptAtEnd });
			#endregion

			html.Controls.Add(head);
			html.Controls.Add(body);
			Controls.Add(html);
		}

		void Page_Load() {
			rptMain.DataSource = ListEntries;
			rptMain.DataBind();

			var path = VirtualPathUtility.AppendTrailingSlash(Context.Request.Path);
			string parentPath;
			if (path.Equals("/") || path.Equals(VirtualPathUtility.AppendTrailingSlash(HttpRuntime.AppDomainAppVirtualPath))) {
				parentPath = null;
			} else {
				parentPath = VirtualPathUtility.Combine(path, "..");
				if (!IsRootAllowed && parentPath.Equals("/")) parentPath = null;
			}
			if (string.IsNullOrEmpty(parentPath)) {
				hlNavigateUp.Visible = false;
				hlNavigateUp.Enabled = false;
			} else {
				hlNavigateUp.NavigateUrl = parentPath;
			}
		}

		private void RptMain_ItemDataBound(object sender, RepeaterItemEventArgs e) {
			var data = e.Item.DataItem as ListEntry;
			if (data == null) return;
			var item = e.Item;
			var tr = new HtmlTableRow { };

			tr.Cells.Add(new HtmlTableCell { InnerHtml = string.Format("<i class=\"far fa-{0}\"></i><a href=\"{1}\">{2}</a>", GetIconCss(data), GetUrl(data), data.Filename) });
			tr.Cells.Add(new HtmlTableCell { InnerText = data.FileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss") });
			tr.Cells.Add(new HtmlTableCell { InnerText = data.FileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") });
			tr.Cells.Add(new HtmlTableCell { InnerText = !data.IsDirectory ? GetSize(data.FileInfo) : "" });
			item.Controls.Add(tr);

		}


		#region Methods
		private string GetPath() {
			string currentFolder = "";
			string[] foldersArray = Context.Request.Path.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (foldersArray.Length > 0) currentFolder = foldersArray[foldersArray.Length - 1];
			if (string.IsNullOrWhiteSpace(currentFolder)) currentFolder = Context.Request.Url.AbsolutePath;
			return currentFolder;
		}

		private string GetSize(FileSystemInfo info) {
			string result = "Not Available";
			string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB" };

			if (info is FileInfo) {
				var fi = info as FileInfo;
				long size = fi.Length;
				int counter = 0;
				double dSize = size; ;
				while (dSize > 1024) {
					dSize /= 1024;
					counter++;
				}
				result = string.Format("{0:.##} {1}", dSize, SizeSuffixes[counter]);
			}
			return result;
		}

		private object GetIconCss(ListEntry entry) {
			var path = entry.Path;
			string iconCss = "";
			if (entry.IsDirectory) {
				iconCss = "folder";
			} else {
				string extn = Path.GetExtension(path);
				switch (extn) {
					case ".pdf": iconCss = "file-pdf"; break;
					case ".docx":
					case ".doc": iconCss = "file-word"; break;
					case ".xlsx":
					case ".xls": iconCss = "file-excel"; break;
					case ".txt": iconCss = "file-alt"; break;
					default: iconCss = "file"; break;
				}
			}
			return iconCss;
		}

		private string GetUriPrefix(string fileExtension) {
			fileExtension = fileExtension.ToLower();
			if (fileExtension.Equals(".docx") || fileExtension.Equals(".doc")) return "ms-word:ofv|u|";
			if (fileExtension.Equals(".xls") || fileExtension.Equals(".xlsx")) return "ms-excel:ofv|u|";
			if (fileExtension.Equals(".ppt") || fileExtension.Equals(".pptx")) return "ms-powerpoint:ofv|u|";
			return "";
		}

		private object GetUrl(ListEntry entry) {
			string extension = Path.GetExtension(entry.Path);
			string uriPrefix = GetUriPrefix(extension);
			string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
			string hyperlink = uriPrefix + baseUrl + entry.VirtualPath;
			return hyperlink;
		}
		#endregion
	}

	#region HTML Tag Classes
	class HtmlHtml : HtmlContainerControl {
		public HtmlHtml() : base("html") { }
	}

	class HtmlBody : HtmlContainerControl {
		public HtmlBody() : base("body") { }
	}

	class HtmlStyle : HtmlContainerControl {
		public HtmlStyle() : base("style") { }
	}
	class HtmlH2 : HtmlContainerControl {
		public HtmlH2() : base("h2") { }
	}
	class HtmlScript : HtmlContainerControl {
		public string Src { get; set; }
		public HtmlScript() : base("script") { }
		protected override void Render(HtmlTextWriter writer) {
			writer.Write("<script");
			if (!string.IsNullOrWhiteSpace(Src)) writer.Write(" src=\"{0}\"", Src);
			writer.Write(">");
			if (!String.IsNullOrWhiteSpace(InnerHtml)) {
				writer.WriteLine();
				writer.Write(InnerHtml);
				writer.WriteLine();
			}
			writer.Write("</script>");
		}
	}
	#endregion
}
