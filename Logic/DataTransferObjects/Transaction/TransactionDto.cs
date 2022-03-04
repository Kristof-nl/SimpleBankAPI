using Logic.DataTransferObjects.BankAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.DataTransferObjects.Transaction
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime TransactionDate { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public double TransactionAmount { get; set; }
        public double AmountBefore { get; set; }
        public double AmountAfter { get; set; }
    }
}
