using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject_SusanBoratynska.Models.ViewModels
{
    public class ShowTeam
    {
        // Grab specific Team:
        public virtual Team Team { get; set; }

        // Show list of Players on specific Team
        public List<Player> Players { get; set; }
    }
}