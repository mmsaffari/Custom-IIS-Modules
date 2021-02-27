using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MMS.BetterDirectoryListing {
	class Template : Page {

		Repeater DirectoryListing = new Repeater();
		//Label FileCount = new Label();

		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			DirectoryListing
		}
		void Page_Load() {
			using (var writer = new HtmlTextWriter(this.Response.Output)) {
				writer.Write(Properties.Resources.Page);
				DirectoryListing.RenderControl(writer);
			}

		}
	}
}
