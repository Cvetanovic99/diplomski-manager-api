using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Exceptions
{
    public class ApiException : Exception
    {
        public int Code { get; }

        public ApiException(string message, int code) : base(message)
        {
            Code = code;
        }
    }
}
