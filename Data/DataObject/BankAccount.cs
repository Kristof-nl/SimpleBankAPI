using CrossCuttingConcerns.Generics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataObjects
{
    public class BankAccount : IEntity
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public double AccountBalance { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
