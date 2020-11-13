using planinarskoUdruzenjeV3.Areas.Identity.Data;
using System;
using System.Collections.Generic;

namespace planinarskoUdruzenjeV3.Models
{
    public partial class Rate
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Event Event { get; set; }
        public virtual User User { get; set; }
    }
}
