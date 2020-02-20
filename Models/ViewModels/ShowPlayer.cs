using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject_SusanBoratynska.Models.ViewModels
{
    public class ShowPlayer
    {
        // Grab specific Player:
        public virtual Player Player { get; set; }

        // List of every Position they play:
        public List<Position> Positions { get; set; }

        public List<Team> Teams { get; set; }

    }
}