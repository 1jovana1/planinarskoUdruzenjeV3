using System;
using System.Collections.Generic;

namespace planinarskoUdruzenjeV3.Models
{
    public partial class Participation
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public short? IsApproved { get; set; }

        public virtual Event Event { get; set; }
    }
}
