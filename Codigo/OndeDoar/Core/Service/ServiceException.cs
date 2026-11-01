using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class ServiceException : SystemException
    {
        public ServiceException(string messagem, Exception inner) : base(messagem, inner)
        {

        }
    }
}
