using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.DataTransferObjects.BankAccount
{
    public class CreateBankAccountDto
    {
        public DateTime CreationDate { get; set; }
        public double AccountBalance { get; set; }
    }
}
