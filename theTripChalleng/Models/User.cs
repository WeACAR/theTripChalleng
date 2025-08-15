using System;
using System.Collections.Generic;

namespace theTripChalleng.Models
{
    public partial class User
    {
        public User()
        {
            EmailReceiverNavigations = new HashSet<Email>();
            EmailSenderNavigations = new HashSet<Email>();
            Nicknames = new HashSet<Nickname>();
            PointRequests = new HashSet<PointRequest>();
            PointsHistories = new HashSet<PointsHistory>();
        }

        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public long? TotalPoints { get; set; }
        public string? Password { get; set; }
        public long? RuleId { get; set; }
        public byte[]? Image { get; set; }

        public virtual Rule? Rule { get; set; }
        public virtual ICollection<Email> EmailReceiverNavigations { get; set; }
        public virtual ICollection<Email> EmailSenderNavigations { get; set; }
        public virtual ICollection<Nickname> Nicknames { get; set; }
        public virtual ICollection<PointRequest> PointRequests { get; set; }
        public virtual ICollection<PointsHistory> PointsHistories { get; set; }
    }
}
