using System;
using System.Collections.Generic;

namespace theTripChalleng.Models;

public partial class RequestStatus
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<PointRequest> PointRequests { get; set; } = new List<PointRequest>();
}
