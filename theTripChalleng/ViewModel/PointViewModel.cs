//make full ViewModel for Point models where any attribute that has id in its name will be a foreign key and need to get the name of the foreign key (one view model for each Point model)
using System.Collections.Generic;
using theTripChalleng.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System;

namespace theTripChalleng.ViewModel
{

    //view model for PointRequest
    public class PointRequestViewModel
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public string? UserName { get; set; }

        public long CriterionId { get; set; }
        public string? CriterionName { get; set; }
        public long RequestedPoints { get; set; }
        public string? Proof { get; set; }
        public string? AdminComment { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Status { get; set; } // Use enum for status if needed
        public string? StatusName { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Criterion Criterion { get; set; } = null!;
    }

    //view model for PointsHistory
    public class PointsHistoryViewModel
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public string? UserName { get; set; }

        public long CriterionId { get; set; }
        public string? CriterionName { get; set; }

        public DateTime CreatedAt { get; set; }

        public long Points { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Criterion Criterion { get; set; } = null!;
    }
}