using planinarskoUdruzenjeV3.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace planinarskoUdruzenjeV3.Models
{
    [NotMapped]
    public class Participant
    {
        public string UserId;
        public string Name;
        public bool isApproved;
    }
}
