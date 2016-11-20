using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException():base("You need Admin rights to execute this action")
        {
            
        }

        public InvalidTokenException(string message)
            : base(message)
        {
        }

        public InvalidTokenException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
