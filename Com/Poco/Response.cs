using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Com.Poco
{
    public class Response
    {
        public bool success { set; get; }
        public bool isException { set; get; }
        public string exception { set; get; }
        public object body { set; get; }
    }
}
