using System;
using System.Collections.Generic;

namespace theTripChalleng.Models
{
    public partial class Nickname
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? UserId { get; set; }
        public int? CurrectGuess { get; set; }
        public int? WrongGuess { get; set; }
        public int? GotGuessed { get; set; }

        public virtual User? User { get; set; }
    }
}
