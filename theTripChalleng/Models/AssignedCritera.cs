using System;
using System.Collections.Generic;

namespace theTripChalleng.Models;

public partial class AssignedCritera
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? UserId { get; set; }

    public long? CriteriaId { get; set; }
}
