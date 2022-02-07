using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFKIanApi.Exceptions
{
    public class CreateFileException:Exception
    {
        public CreateFileException(string message,Exception ex):base(message,ex)
        {

        }

       
    }

    public class TaskUrlBuilderException : Exception
    {
        public TaskUrlBuilderException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
