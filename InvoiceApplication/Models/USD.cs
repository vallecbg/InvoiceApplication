using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceApplication.Models
{
    public class USD : Currency
    {
        public USD() : base("US Dollar", "USD", 1.0m) { }
    }
}
