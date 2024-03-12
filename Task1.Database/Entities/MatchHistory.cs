using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.Database.Enums;

namespace Task1.Database.Entities
{
    public class MatchHistory
    {
        [Key]
        public int Id { get; set; }
        public int Sum { get; set; }
        public User? Player1 { get; set; }
        public MatchActions? Player1Action {  get; set; }
        public User? Player2 { get; set; }
        public MatchActions? Player2Action { get; set; }
        public MatchStatus Status { get; set; }
        public GameTransactions? Transactions { get; set; }
    }
}
