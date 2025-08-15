using System;
using System.Collections.Generic;

namespace theTripChalleng.Models
{
    public partial class Rule
    {
        public Rule()
        {
            Users = new HashSet<User>();
        }

        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? RuleName { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
