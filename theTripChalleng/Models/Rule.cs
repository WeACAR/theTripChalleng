using System;
using System.Collections.Generic;

namespace theTripChalleng.Models;

public partial class Rule
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? RuleName { get; set; }

    public virtual ICollection<User1> User1s { get; set; } = new List<User1>();
}
