using System;
using System.Collections.Generic;

namespace theTripChalleng.Models
{
    public partial class PointRequest
    {
        public long RequestId { get; set; }
        public DateTime CreatedAt { get; set; }
        public long UserId { get; set; }
        public long? CriteriaId { get; set; }
        public long? RequestedPoints { get; set; }
        public string? Proof { get; set; }
        public long? StatusId { get; set; }
        public string? AdminComment { get; set; }

        public virtual Criterion? Criteria { get; set; }
        public virtual RequestStatus? Status { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
