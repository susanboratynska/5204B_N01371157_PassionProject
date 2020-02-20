using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PassionProject_SusanBoratynska.Data;
using PassionProject_SusanBoratynska.Models;
using PassionProject_SusanBoratynska.Models.ViewModels;
using System.Diagnostics;

namespace PassionProject_SusanBoratynska.Controllers
{
    public class TeamController : Controller
    {
        private VolleyballContext db = new VolleyballContext();
        // Get Teams
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(string searchkey, string men, string women, string all)
        {
            Debug.WriteLine("Search Key: " + searchkey);
            // SQL query: List of Teams
            List<Team> myteam = db.Teams.SqlQuery("SELECT * FROM teams ORDER BY name").ToList();

            if (searchkey != "")
            {
                // Search by name
                myteam = db.Teams.SqlQuery("SELECT * FROM Teams WHERE name LIKE '%" + searchkey + "%' OR city LIKE '%" + searchkey + "%' OR state LIKE '$" + searchkey +"%' ORDER BY name;").ToList();
                Debug.WriteLine("myteam: " + myteam);
            }

            if ( men == "Men")
            {
                myteam = db.Teams.SqlQuery("SELECT * FROM Teams WHERE division LIKE '" + men + "%' ORDER BY name;").ToList();
                Debug.WriteLine("myteam: " + myteam);
            }

            if (women == "Women")
            {
                myteam = db.Teams.SqlQuery("SELECT * FROM Teams WHERE division LIKE '" + women + "%' ORDER BY name;").ToList();
                Debug.WriteLine("myteam: " + myteam);
            }

            if (all == "All")
            {
                myteam = db.Teams.SqlQuery("SELECT * FROM Teams").ToList();
                Debug.WriteLine("myteam: " + myteam);
            }

            return View(myteam);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string TeamName, string TeamCity, string TeamState, string TeamDivision)
        {
            string query = "INSERT INTO teams (name, city, state, division) values (@TeamName, @TeamCity, @TeamState, @TeamDivision)";
            SqlParameter[] sqlparams = new SqlParameter[4];
            sqlparams[0] = new SqlParameter("@TeamName", TeamName);
            sqlparams[1] = new SqlParameter("@TeamCity", TeamCity);
            sqlparams[2] = new SqlParameter("@TeamState", TeamState);
            sqlparams[3] = new SqlParameter("@TeamDivision", TeamDivision);
            Debug.WriteLine("Query: " + query);

            db.Database.ExecuteSqlCommand(query, sqlparams);
            return RedirectToAction("List");
        }

        public ActionResult Show(int id)
        {
            //// Retrieve data from a specific Team:
            //string query = "SELECT * FROM teams WHERE teamID = @id";
            //var parameter = new SqlParameter("@id", id);
            //Team team = db.Teams.SqlQuery(query, parameter).FirstOrDefault();

            //Debug.WriteLine("Query: " + query);

            //return View(team);

            // Retrieve data from a specific Team:
            string team_query = "SELECT * FROM teams WHERE teamID = @id";
            var parameter = new SqlParameter("@id", id);
            Team team = db.Teams.SqlQuery(team_query, parameter).FirstOrDefault();

            Debug.WriteLine("Query: " + team_query);

            // Find all Players that play on a specific Team
            string player_query = "SELECT * FROM players WHERE teamID=@id";
            var fk_parameter = new SqlParameter("@id", id);
            List<Player> PlayerRoster = db.Players.SqlQuery(player_query, fk_parameter).ToList();

            //ViewModels:
            // 1. Show team data
            // 2. Show all players on that team

            ShowTeam viewmodel = new ShowTeam();
            viewmodel.Team = team;
            viewmodel.Players = PlayerRoster;

            return View(viewmodel);
        }

        public ActionResult Update(int id)
        {
            string query = "SELECT * FROM teams WHERE teamID = @id";
            var parameter = new SqlParameter("@id", id);
            Team selectedteam = db.Teams.SqlQuery(query, parameter).FirstOrDefault();

            return View(selectedteam);
        }
        [HttpPost]
        public ActionResult Update(int id, string TeamName, string TeamCity, string TeamState, string TeamDivision)
        {
            string query = "UPDATE teams SET name=@TeamName, city=@TeamCity, state=@TeamState, division=@TeamDivision WHERE teamID = @id";
            SqlParameter[] sqlparams = new SqlParameter[5];
            sqlparams[0] = new SqlParameter("@TeamName", TeamName);
            sqlparams[1] = new SqlParameter("@TeamCity", TeamCity);
            sqlparams[2] = new SqlParameter("@TeamState", TeamState);
            sqlparams[3] = new SqlParameter("@TeamDivision", TeamDivision);
            sqlparams[4] = new SqlParameter("@id", id);
            db.Database.ExecuteSqlCommand(query, sqlparams);

            return RedirectToAction("List");
        }

        public ActionResult DeleteConfirm(int id)
        {
            string query = "SELECT * FROM teams where teamID=@id";
            SqlParameter param = new SqlParameter("@id", id);
            Team selectedteam = db.Teams.SqlQuery(query, param).FirstOrDefault();
            return View(selectedteam);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            string query = "DELETE FROM teams WHERE teamID=@id";
            SqlParameter param = new SqlParameter("@id", id);
            db.Database.ExecuteSqlCommand(query, param);


            //for the sake of referential integrity, unset the species for all pets
            string refquery = "UPDATE players SET teamID = '' WHERE teamID=@id";
            db.Database.ExecuteSqlCommand(refquery, param); //same param as before

            return RedirectToAction("List");
        }

    }
}

