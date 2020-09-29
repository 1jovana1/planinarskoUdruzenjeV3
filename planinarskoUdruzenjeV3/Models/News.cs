using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace planinarskoUdruzenjeV3.Models
{
    public partial class News
    {
        public News()
        {
            File = new HashSet<File>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<File> File { get; set; }

        [NotMapped]
        public static string[] GetCategoryList = { "Aktuelno", "Novosti" };
    }
}
