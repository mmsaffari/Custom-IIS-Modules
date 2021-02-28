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

		//Repeater DirectoryListing = new Repeater();
		//Label FileCount = new Label();

		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			Controls.Clear();
			var html = new HtmlElement();
			var head = new HtmlHead();
			head.Title= string.Format("Directory contents of {0}", Context.Request.Path);
			var linkDataTablesCSS = new HtmlLink() { Href = "/static/datatables/css/jquery.dataTables.min.css" };
			linkDataTablesCSS.Attributes.Add("rel", "stylesheet");
			linkDataTablesCSS.Attributes.Add("type", "text/css");
			head.Controls.Add(linkDataTablesCSS);
			var linkFontAwesomeCSS = new HtmlLink() { Href = "/static/fa/fontawesome-all.css" };
			linkFontAwesomeCSS.Attributes.Add("rel", "stylesheet");
			linkFontAwesomeCSS.Attributes.Add("type", "text/css");
			head.Controls.Add(linkFontAwesomeCSS);

			var body = new BodyElement();
			html.Controls.Add(head);
			html.Controls.Add(body);
			Controls.Add(html);
		}
		void Page_Load() {
			ListEntryCollection listEntries = Context.Items[DirectoryBrowsingModule.DirectoryBrowsingContextKey] as ListEntryCollection;
			

		}
	}

	class HtmlElement : HtmlContainerControl {
		public HtmlElement() : base("html") { }
	}

	class BodyElement : HtmlContainerControl {
		public BodyElement() : base("body") { }
	}
}
