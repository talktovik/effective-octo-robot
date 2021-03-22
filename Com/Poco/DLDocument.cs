﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Com.Poco
{
    public class DLDocument
    {
        public int documentId { set; get; }
        public int docTypeId { set; get; }
        public int companyId { set; get; }
        public string documentPath { set; get; }
        public DateTime documentDate { set; get; }
        public string sender { set; get; }
        public int rev_no { set; get; }

    }
}
