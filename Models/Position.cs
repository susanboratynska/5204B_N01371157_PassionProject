using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassionProject_SusanBoratynska.Models
{
    public class Position
    {
        [Key]
        public int positionID { get; set; }
        public string position { get; set; }

        // Many players to many positions
        public virtual ICollection<Player> Players { get; set; }
    }
}