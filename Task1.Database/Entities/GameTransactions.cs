using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.Database.Enums;

namespace Task1.Database.Entities
{
    public class GameTransactions
    {
        [Key]
        public int Id { get; set; }
        public required User User { get; set; }
        public TransactionTypes TransactionType { get; set; }
        public int Sum { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
