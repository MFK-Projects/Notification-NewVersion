using System;

namespace MFKIanApi.Exceptions
{
    public class TaskUrlBuilderException : Exception
    {
        public TaskUrlBuilderException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
