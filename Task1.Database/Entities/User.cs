using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.Database.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public required string Login { get; set; }
    }
}
