using System;
using System.Collections.Generic;

namespace theTripChalleng.Models;

public partial class Category
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();

    public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();
}
