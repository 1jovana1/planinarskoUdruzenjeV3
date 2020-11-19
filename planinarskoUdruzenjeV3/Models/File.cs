using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace planinarskoUdruzenjeV3.Models
{
    public partial class File
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string FileName { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public int? EventId { get; set; }
        public int? NewsId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public bool IsImage()
        {

            return this.ContentType.Contains("image");
        }

        public Event Event { get; set; }
        public News News { get; set; }

    }
}
