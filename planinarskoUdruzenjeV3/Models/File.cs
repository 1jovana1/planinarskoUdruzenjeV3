using System;
using System.Collections.Generic;

namespace planinarskoUdruzenjeV3.Models
{
    public partial class File
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public int? EventId { get; set; }
        public int? NewsId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Event Event { get; set; }
        public News News { get; set; }
    }
}
