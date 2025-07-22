using System;
using System.Collections.Generic;

namespace theTripChalleng.Models;

public partial class PointsHistory
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? UserId { get; set; }

    public long? CriteriaId { get; set; }

    public long? Points { get; set; }

    public string? ApprovedBy { get; set; }

    public virtual Criterion? Criteria { get; set; }

    public virtual User1? User { get; set; }
}
