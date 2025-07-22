using System;
using System.Collections.Generic;

namespace theTripChalleng.Models;

public partial class Criterion
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? MaxPoints { get; set; }

    public bool? PointsType { get; set; }

    public long? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<PointRequest> PointRequests { get; set; } = new List<PointRequest>();

    public virtual ICollection<PointsHistory> PointsHistories { get; set; } = new List<PointsHistory>();
}
