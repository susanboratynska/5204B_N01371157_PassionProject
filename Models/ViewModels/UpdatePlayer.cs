using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject_SusanBoratynska.Models.ViewModels
{
    public class UpdatePlayer
    {
        // Need Position and Player info to update the Player
        public Player Player { get; set; }
        public List<Team> Teams { get; set; }
        public virtual ICollection<Position> Positions { get; set; }
    }
}