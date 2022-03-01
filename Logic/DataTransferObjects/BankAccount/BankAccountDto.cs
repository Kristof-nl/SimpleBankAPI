using Logic.DataTransferObjects.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.DataTransferObjects.BankAccount
{
    public class BankAccountDto
    {
        public string AccountNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public double AccountBalance { get; set; }
        public ICollection<TransactionDto> Transactions { get; set; }
    }
}
