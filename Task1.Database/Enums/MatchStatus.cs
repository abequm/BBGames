using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.Database.Enums
{
    public enum MatchStatus
    {
        Created,
        Awaiting,
        InGame,
        Player1Win,
        Player2Win,
        Draw // Ничья
    }
}
