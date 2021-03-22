﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Com.Poco
{
    public class Settings
    {
        // This class is for performing crud operation on settings tables and take their values in respective text fields
        public string docRoot { set; get; }
        public string refName { set; get; }
        public string templateRoot { set; get; }
        public string serviceInvoiceNo { set; get; }
        public string taxInvoiceNo { set; get; }
    }
}
