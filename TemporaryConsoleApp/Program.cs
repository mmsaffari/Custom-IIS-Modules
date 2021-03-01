using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemporaryConsoleApp {
	class Program {
		static void Main(string[] args) {
			new System.IO.DirectoryInfo(@"C:\Workspace\VS\Custom IIS Modules\BetterDirectoryListing\Resources")
				.GetFiles("*.*", System.IO.SearchOption.AllDirectories)
				.GroupBy(f => f.Extension)
				.Select(g => new { Ext = g.Key, Sample = g.First().FullName })
				.ToList()
				.ForEach(fi => {
					//Console.WriteLine("{0} - {1}", fi.Ext, fi.Sample);
					var c = System.Web.MimeMapping.GetMimeMapping(fi.Sample);
					Console.WriteLine(c);
				});
		}
	}
}
