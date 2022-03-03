using CrossCuttingConcerns.Generics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataObjects
{
    public class Transaction : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime TransactionDate { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public double TransactionAmount { get; set; }
        public double AmountBefore { get; set; }
        public double AmountAfter { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}
