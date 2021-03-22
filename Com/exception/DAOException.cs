using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QM.Com.exception
{
    public class DAOException: Exception
    {
        public DAOException(string message) : base(message)
        {

        }
    }
}
