using System;
using System.Collections.Generic;

namespace theTripChalleng.Models
{
    public partial class Reward
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long? MinPoints { get; set; }
        public byte[]? Image { get; set; }
        public long? CategoryId { get; set; }

        public virtual Category? Category { get; set; }
    }
}
