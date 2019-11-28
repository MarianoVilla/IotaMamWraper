using IOTAAPI.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IotaMamConnection conn;

            // *************** Notes. *************** \\

            //Instantiating.
            //
            conn = new IotaMamConnection("https://nodes.devnet.iota.org:443");
            conn.Write("SomeMessage");
            var a = conn.WriteAndGetState("SomeOtherMessage");


            var p = conn.GetPublishedMessages();
        }
    }
}
