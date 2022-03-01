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
        public double Ammount { get; set; }
        public double AmmountBefore { get; set; }
        public double AammountAfter { get; set; }
        public BankAccount BankAccount { get; set; }

    }
}
