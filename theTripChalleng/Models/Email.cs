using System;
using System.Collections.Generic;

namespace theTripChalleng.Models
{
    public partial class Email
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? Sender { get; set; }
        public long? Receiver { get; set; }
        public string? Message { get; set; }

        public virtual User? ReceiverNavigation { get; set; }
        public virtual User? SenderNavigation { get; set; }
    }
}
