using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceApplication.Models
{
    public class EUR : Currency
    {
        public EUR() : base("Euro", "EUR", 0.85m) { }
    }
}
