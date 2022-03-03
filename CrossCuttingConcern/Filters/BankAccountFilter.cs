using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCuttingConcern.Filters
{
    public class BankAccountFilter
    {
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }
        public double? AccountBalanceFrom { get; set; }
        public double? AccountBalanceTo { get; set; }
    }
}
