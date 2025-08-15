using System;
using System.Collections.Generic;

namespace theTripChalleng.Models
{
    public partial class Criterion
    {
        public Criterion()
        {
            PointRequests = new HashSet<PointRequest>();
            PointsHistories = new HashSet<PointsHistory>();
        }

        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? MaxPoints { get; set; }
        public bool? PointsType { get; set; }
        public long? CategoryId { get; set; }
        public string? CriteriaName { get; set; }
        public bool? IsAssignable { get; set; }
        public long? MaxAssign { get; set; }
        public long? AssignLeft { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<PointRequest> PointRequests { get; set; }
        public virtual ICollection<PointsHistory> PointsHistories { get; set; }
    }
}
