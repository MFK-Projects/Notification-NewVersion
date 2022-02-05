using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFKianNotificationApi.Expceptions
{
    public class SetApplicationException:Exception
    {
        public SetApplicationException(string message,Exception innerException) : base(message, innerException)
        {

        }
    }

    public class MoreThanOneRecordFindException : Exception
    {
        public MoreThanOneRecordFindException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
