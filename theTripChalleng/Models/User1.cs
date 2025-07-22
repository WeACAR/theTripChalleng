using System;
using System.Collections.Generic;

namespace theTripChalleng.Models;

public partial class User1
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public long? TotalPoints { get; set; }

    public string? Password { get; set; }

    public long? RuleId { get; set; }

    public virtual ICollection<PointRequest> PointRequests { get; set; } = new List<PointRequest>();

    public virtual ICollection<PointsHistory> PointsHistories { get; set; } = new List<PointsHistory>();

    public virtual Rule? Rule { get; set; }
}
