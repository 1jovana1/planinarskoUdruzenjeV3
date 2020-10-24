using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace planinarskoUdruzenjeV3.Models
{
    public class User : IdentityUser { 
    
        public string FirstName { get; set; }
        public string LastName { get; set; }


    }
}
