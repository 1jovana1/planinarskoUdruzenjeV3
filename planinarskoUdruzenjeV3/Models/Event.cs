using System;
using System.Collections.Generic;

namespace planinarskoUdruzenjeV3.Models
{
    public partial class Event
    {
        public Event()
        {
            File = new HashSet<File>();
            Participation = new HashSet<Participation>();
            Rate = new HashSet<Rate>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime Deadline { get; set; }
        public int? MaxParticipanst { get; set; }
        public string Location { get; set; }
        public double? Price { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<File> File { get; set; }
        public ICollection<Participation> Participation { get; set; }
        public ICollection<Rate> Rate { get; set; }

    }
}
