using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace QM.Com.Poco
{
    public class Contact
    {
        public int contactId { set; get; }
        public int companyId { set; get; }
        public string salutation { get; set; }
        
        [DisplayName("Name")]
        public string contact_name { set; get; }
        public string designation { get; set; }
        [DisplayName("Email")]
        public string contact_email { set; get; }
        [DisplayName("Phone")]
        public string contact_phone { set; get; }
        

        
        
        //public string nametoshow { get; set; }
    }
}
