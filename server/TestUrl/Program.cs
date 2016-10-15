using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TestUrl
{
    class Program
    {
        static void Main(string[] args)
        {
            var message = "{sensorType : \"temperature\"}";
            var encoded = HttpUtility.UrlEncode(message);
            
        }
    }
}
