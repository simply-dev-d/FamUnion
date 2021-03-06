﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamUnion.Core.Model
{
    public class Family : ModelBase
    {
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }        
        public string PhotoLocation { get; set; }
        public bool Attending { get; set; }
        public bool ReceivingAlerts { get; set; }

        public virtual IEnumerable<FamilyMember> Members { get; set; }
        public virtual Address Address { get; set; }    
        public virtual IEnumerable<User> Users { get; set; }    // Needed if Members are also linked to users?
    }
}
