using planinarskoUdruzenjeV3.Areas.Identity.Data;
using System;
using System.Collections.Generic;

namespace planinarskoUdruzenjeV3.Models
{
    public partial class Participation
    {
        public const int NOT_EXIST = 2;
        public const int APPROVED = 1;
        public const int NOT_APPROVED = 0;
        public int Id { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public short? IsApproved { get; set; }

        public virtual Event Event { get; set; }
        public virtual User User { get; set; }
    }
}
