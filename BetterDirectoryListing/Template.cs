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
	class Template:Page {

        //Repeater DirectoryListing = new Repeater();
        //Label FileCount = new Label();

		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
            Controls.Add(ParseControl(Properties.Resources.Page));
        }
		void Page_Load() {
            
        }
    }
}
