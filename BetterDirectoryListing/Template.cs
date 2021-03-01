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

		HyperLink hlNavigateUp = new HyperLink();
		Repeater rptMain = new Repeater();
		//Label FileCount = new Label();

		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
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
			rptMain = ParseControl(Properties.Resources.rptMain) as Repeater;
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
			ListEntryCollection listEntries = Context.Items[DirectoryBrowsingModule.DirectoryBrowsingContextKey] as ListEntryCollection;
			bool isRootAllowed = (bool)Context.Items["isRootAllowed"];

			var path = VirtualPathUtility.AppendTrailingSlash(Context.Request.Path);
			string parentPath = null;
			if (path.Equals("/") || path.Equals(VirtualPathUtility.AppendTrailingSlash(HttpRuntime.AppDomainAppVirtualPath))) {
				parentPath = null;
			} else {
				parentPath = VirtualPathUtility.Combine(path, "..");
				if (!isRootAllowed && parentPath.Equals("/")) parentPath = null;
			}
			if (string.IsNullOrEmpty(parentPath)) {
				hlNavigateUp.Visible = false;
				hlNavigateUp.Enabled = false;
			} else {
				hlNavigateUp.NavigateUrl = parentPath;
			}
		}

		#region Methods
		string GetPath() {
			string currentFolder = "";
			string[] foldersArray = Context.Request.Path.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (foldersArray.Length > 0) currentFolder = foldersArray[foldersArray.Length - 1];
			if (string.IsNullOrWhiteSpace(currentFolder)) currentFolder = Context.Request.Url.AbsolutePath;
			return currentFolder;
		}
		#endregion
	}

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
}
