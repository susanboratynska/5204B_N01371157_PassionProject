using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PassionProject_SusanBoratynska.Data
{
    public class VolleyballContext : DbContext
    {
        public VolleyballContext() :base("name=VolleyballContext"){ }

        public System.Data.Entity.DbSet<PassionProject_SusanBoratynska.Models.Player> Players { get; set; }

        public System.Data.Entity.DbSet<PassionProject_SusanBoratynska.Models.Team> Teams { get; set; }
        public System.Data.Entity.DbSet<PassionProject_SusanBoratynska.Models.Position> Positions { get; set; }


    }
}