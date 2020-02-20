using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassionProject_SusanBoratynska.Models
{
    public class Team
    {
        /*
            A Team is comprised of many Players
            Some things that describe a Team:
                - Team Name
                - Team City
                - Team State/Province
                - Division
            A player must reference positions and a team 
        */

        [Key]
        public int teamID { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string division { get; set; }

        // Many players to one Team
        public virtual ICollection<Player> Players { get; set; }
    }
}