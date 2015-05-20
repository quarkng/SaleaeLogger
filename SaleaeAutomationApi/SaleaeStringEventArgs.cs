using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleaeAutomationApi
{
    public class SaleaeStringEventArgs : EventArgs
    {
        public string Value { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        protected SaleaeStringEventArgs( string s )
        {
            Timestamp = DateTime.Now;
            Value = s;
        }
    }

    public class SeleaeReadEventArgs : SaleaeStringEventArgs
    {
        public SeleaeReadEventArgs( string s ) : base(s)
        {
        }
    }

    public class SeleaeWriteEventArgs : SaleaeStringEventArgs
    {
        public SeleaeWriteEventArgs(string s) : base(s)
        {
        }
    }
}
