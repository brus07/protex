using System;

namespace ProtexCore.Config
{
    public class WrongXMLSyntaxException : ApplicationException
    {
        public WrongXMLSyntaxException()
        {
        }

        public WrongXMLSyntaxException(string message)
            : base(message)
        {
        }

        public WrongXMLSyntaxException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}