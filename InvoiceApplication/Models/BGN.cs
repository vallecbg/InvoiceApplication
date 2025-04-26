using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceApplication.Models
{
    public class BGN : Currency
    {
        public BGN() : base("Bulgarian Lev", "BGN", 1.95m) { }
    }
}
