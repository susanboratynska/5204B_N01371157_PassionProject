using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PassionProject_SusanBoratynska.Models
{
    public class Player
    {
        /*
            A player play a position(s) and also plays on a team
            Some things that describe a player:
                - First Name
                - Last Name
                - Division
            A player must reference positions and a team 
        */

        [Key]
        public int playerID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }

        //// profilePic can be 0 or 1;
        //// (1) = has a picture
        //// (0) = no picture
        public int? hasPic { get; set; }
        //// .jpeg, .jpeg, .png
        public string picExtension { get; set; }


        // One Team to many Players
        public int teamID { get; set; }
        [ForeignKey("teamID")]
        public virtual Team team { get; set; }

        // Many Players to many Positions
        public virtual ICollection<Position> Positions { get; set; }
    }
}