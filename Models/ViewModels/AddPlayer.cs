using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject_SusanBoratynska.Models.ViewModels
{
    public class AddPlayer
    {
        // List of every Position they play:
        public List<Position> Positions { get; set; }

        public List<Team> Teams { get; set; }
    }
}