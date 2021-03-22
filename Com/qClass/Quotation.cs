using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.Com.qClass
{
    public class Quotation
    {
        public string companyName { get; set; }
        public string referenceID { get; set; }
        public int revisionID { get; set; }
        public string dateTime { get; set; }
        

        public int quotationID { get; set; }
        public string  senderName { get; set; }
        
        public int companyId { get;  set; }
        
        public string yourEnquiry { get; set; }
        public string contactNameone { get; set; }
        public string  contactNametwo  { get; set; }
        public string firstMail { get; set; }
        public string secondMail { get; set; }
        public string templatepath { get; set; }
        public string wordFileLocation { get; set; }
        public string pdfFileLocation { get; set; }
    }
}
