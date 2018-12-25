using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp
{
    public class _URI
    {
        public static void MakRelative()
        {
            var from = "e:/cd/abc";
            var to = "e:/cd";
            var fromUri = new System.Uri(from);
            var toUri = new System.Uri(to);

            Console.WriteLine("The difference is {0}", fromUri.MakeRelativeUri(toUri));
            var relativePath = System.Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString());
            Console.WriteLine(relativePath);
        }
      
    }
}