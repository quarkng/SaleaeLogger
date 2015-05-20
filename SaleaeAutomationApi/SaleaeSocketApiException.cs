using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleaeAutomationApi
{
    [Serializable]
    class SaleaeSocketApiException : Exception
    {
        public SaleaeSocketApiException()
            : base()
        {
        }

        public SaleaeSocketApiException(string message)
            : base(message)
        {
        }

        public SaleaeSocketApiException(string message, Exception inner_exception)
            : base(message, inner_exception)
        {
        }

    }
}
