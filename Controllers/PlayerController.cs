﻿using System;
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
using System.IO; // Add namespace to allow file upload for ProfilePic

namespace PassionProject_SusanBoratynska.Controllers
{
    public class PlayerController : Controller
    {
        private VolleyballContext db = new VolleyballContext();

        // Get list of Players:
        public ActionResult List(string searchkey)
        {
            Debug.WriteLine("Search Key: " + searchkey);

            // SQL query: List of all Players ordered by Firstname
            List<Player> myplayers = db.Players.SqlQuery("SELECT * FROM Players ORDER BY firstname").ToList();

            if (searchkey != "")
            {
                // Search by name
                myplayers = db.Players.SqlQuery("SELECT * FROM Players WHERE firstname LIKE '%" + searchkey + "%' OR lastname LIKE '%" + searchkey + "%' ORDER BY firstname;").ToList();
                Debug.WriteLine("myplayers: " + myplayers);
            }

            return View(myplayers);
        }

        // Show a specific Player:
        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.SqlQuery("SELECT * FROM Players WHERE playerID=@playerID", new SqlParameter("@playerID", id)).FirstOrDefault();
            if (player == null)
            {
                return HttpNotFound();
            }

            // Retrieve list of Positions each Player plays
            string query = "SELECT * FROM Positions INNER JOIN PositionPlayers on Positions.positionID = PositionPlayers.Position_positionID WHERE Player_playerID = @id";
            SqlParameter param = new SqlParameter("@id", id);
            List<Position> PositionPlayers = db.Positions.SqlQuery(query, param).ToList();

            ShowPlayer viewmodel = new ShowPlayer();
            viewmodel.Player = player;
            viewmodel.Positions = PositionPlayers;
            
            return View(viewmodel);
        }

        //[HttpPost]
        //public ActionResult AttachPosition(int id, int positionID)
        //{
        //    Debug.WriteLine("Player ID is " + id + " and the Position ID is " + positionID);

        //    string check_query = "SELECT * FROM Positions INNER JOIN PositionPlayers on PositionPlayers.Position_positionID = Positions.positionID WHERE Position_positionID=@positionID and Player_playerID=@id";
        //    SqlParameter[] check_params = new SqlParameter[2];
        //    check_params[0] = new SqlParameter("@id", id);
        //    check_params[1] = new SqlParameter("@positionID", positionID);
        //    List<Position> positions = db.Positions.SqlQuery(check_query, check_params).ToList();
            
        //    // Execute only if that player doesn't already play that position:
        //    if (positions.Count <= 0)
        //    {
        //        string query = "INSERT INTO PositionPlayers (Position_PositionID, Player_PlayerID) VALUES (@positionID, @id)";
        //        SqlParameter[] sqlparams = new SqlParameter[2];
        //        sqlparams[0] = new SqlParameter("@id", id);
        //        sqlparams[1] = new SqlParameter("@positionID", positionID);

        //        db.Database.ExecuteSqlCommand(query, sqlparams);
        //    }
        //    return RedirectToAction("Show/" + id);
        //}



        public ActionResult Add()
        {

            // The Add.cshtml needs a list of Teams:
            var team_query = "SELECT * FROM teams ORDER BY name";
            List<Team> teams = db.Teams.SqlQuery(team_query).ToList();

            // The Add.cshtml needs a list of Positions:
            var position_query = "SELECT * FROM Positions";
            List<Position> positions = db.Positions.SqlQuery(position_query).ToList();

            AddPlayer AddPlayerViewModel = new AddPlayer();
            AddPlayerViewModel.Teams = teams;
            AddPlayerViewModel.Positions = positions;
           
            return View(AddPlayerViewModel);

        }


        [HttpPost]
        public ActionResult Add(string firstname, string lastname, int teamID, HttpPostedFileBase ProfilePic, List<int> positionArray)
        {

            //STEP 1: PULL DATA! The data is access as arguments to the method. Make sure the datatype is correct!
            //The variable name  MUST match the name attribute described in Views/Pet/Add.cshtml in the view

            //Tests are very useul to determining if you are pulling data correctly!
            //Debug.WriteLine("Want to create a pet with name " + PetName + " and weight " + PetWeight.ToString()) ;

            //STEP 2: FORMAT QUERY! the query will look something like "insert into () values ()"...
            string query = "INSERT INTO players (firstname, lastname, teamID) VALUES (@FirstName, @LastName, @teamID)"; 
            SqlParameter[] sqlparams = new SqlParameter[3]; //0,1,2,3,4 pieces of information to add // CREATE AN ARRAY, EACH OF THE ITEMS IS GOING TO CORRESPOND TO THE COLUMNS
            //each piece of information is a key and value pair
            sqlparams[0] = new SqlParameter("@FirstName", firstname); //MATCHES LINES 78
            sqlparams[1] = new SqlParameter("@LastName", lastname);
            sqlparams[2] = new SqlParameter("@teamID", teamID);

            //db.Database.ExecuteSqlCommand will run insert, update, delete statements
            //db.Pets.SqlCommand will run a select statement, for example.
            //------
            //db.Database.ExecuteSqlCommand(query, sqlparams); // TAKES IN 1 ARGRUMENTS, ONE OF THEM IS THE QUERY (LINE 81), THE SECOND IS THE SQL PARAMS (AN ARRAY OF SQL PARAMS)

            // If the above line inserts into the database then we have a PlayerID because db.Database.ExecuteSqlCommand() returns an Int32
            // Therefore, we can add the positions to the PositionPlayer bridging table:
            int numRowAffected = db.Database.ExecuteSqlCommand(query, sqlparams);
            if (numRowAffected == 1)
            {
            
                Player player = db.Players.SqlQuery("SELECT * FROM players ORDER BY playerID DESC").First();
                int playerID = player.playerID;

                foreach (var position in positionArray)
                {
                    
                    // Add Positions to bridging table (PositionPlayers)
                    string bridging_query = "INSERT INTO PositionPlayers (Player_playerID, Position_positionID) VALUES (" + playerID + ", @position)";
                    SqlParameter[] parameter = new SqlParameter[2];
                    parameter[0] = new SqlParameter("@id", playerID);
                    parameter[1] = new SqlParameter("@position", position);
                    db.Database.ExecuteSqlCommand(bridging_query, parameter);
                }

                // Add Profile Picture to Player
                // SRC: Christine Bittle
                int hasPic = 0;
                string picExtension = "";
                // Check to see if Picture exists
                if (ProfilePic != null)
                {
                    Debug.WriteLine("File identified");

                    // Check if file size is greater than 0 bytes
                    if (ProfilePic.ContentLength > 0)
                    {
                        Debug.WriteLine("Identified image");
                        var filetypes = new[] { "jpeg", "jpg", "png" };
                        var extension = Path.GetExtension(ProfilePic.FileName).Substring(1);

                        if (filetypes.Contains(extension))
                        {
                            try
                            {
                                // Make the file name the id of the image
                                string filename = playerID + "." + extension;

                                // Create a direct file path to ~/Content/ProfilePics/{id}.{extension}
                                string path = Path.Combine(Server.MapPath("~/Content/ProfilePics/"), filename);

                                // Save file
                                ProfilePic.SaveAs(path);

                                // If all the above holds true, set the following:
                                hasPic = 1;
                                picExtension = extension;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Profile Picture was not saved.");
                                Debug.WriteLine("Exception: " + ex);
                            }
                        }
                    }

                    string profile_query = "UPDATE players SET hasPic=@hasPic, picExtension=@picExtension WHERE playerID=@id";
                    SqlParameter[] parameters = new SqlParameter[3];

                    Debug.WriteLine(profile_query);

                    parameters[0] = new SqlParameter("@id", playerID);
                    parameters[1] = new SqlParameter("@hasPic", hasPic);
                    parameters[2] = new SqlParameter("@picExtension", picExtension);

                    Debug.WriteLine("Query: " + profile_query);

                    db.Database.ExecuteSqlCommand(profile_query, parameters);


                }


            }

            return RedirectToAction("List"); //GO TO THE LIST OF PETS SO I CAN SEE THE PET I JUST ADDED, IT WILL GO THE FUNCTION CALL LIST ON LINE 46
        }

        public ActionResult Update(int id)
        {
            // Retrieve data from a particular Player
            Player selectedplayer = db.Players.SqlQuery("SELECT * FROM players WHERE playerID = @id", new SqlParameter("@id", id)).FirstOrDefault();
            List<Team> Teams = db.Teams.SqlQuery("SELECT * FROM teams ORDER BY name").ToList();
            var position_query = "SELECT * FROM Positions";
            var fk_parameter = new SqlParameter("@id", id);
            List<Position> Positions = db.Positions.SqlQuery(position_query, fk_parameter).ToList();

            UpdatePlayer UpdatePlayerViewModel = new UpdatePlayer();
            UpdatePlayerViewModel.Player = selectedplayer;
            UpdatePlayerViewModel.Teams = Teams;
            UpdatePlayerViewModel.Positions = Positions;

            return View(UpdatePlayerViewModel);
        }


        [HttpPost]
        public ActionResult Update(int id, string FirstName, string LastName, int teamID, HttpPostedFileBase ProfilePic, List<int> positionArray)
        {
            // Add Profile Picture to Player
            // SRC: Christine Bittle
            int hasPic = 0;
            string picExtension = "";
            // Check to see if Picture exists
            if (ProfilePic != null)
            {
                Debug.WriteLine("File identified");

                // Check if file size is greater than 0 bytes
                if (ProfilePic.ContentLength > 0)
                {
                    Debug.WriteLine("Identified image");
                    // File extension: https://www.c-sharpcorner.com/article/file-upload-extension-validation-in-asp-net-mvc-and-javascript/
                    var filetypes = new[] { "jpeg", "jpg", "png" };
                    var extension = Path.GetExtension(ProfilePic.FileName).Substring(1);

                    if (filetypes.Contains(extension))
                    {
                        try
                        {
                            // Make the file name the id of the image
                            string filename = id + "." + extension;

                            // Create a direct file path to ~/Content/Pets/{id}.{extension}
                            string path = Path.Combine(Server.MapPath("~/Content/ProfilePics/"), filename);

                            // Save file
                            ProfilePic.SaveAs(path);

                            // If all the above holds true, set the following:
                            hasPic = 1;
                            picExtension = extension;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Profile Picture was not saved.");
                            Debug.WriteLine("Exception: " + ex);
                        }
                    }
                }

                
            }

            string check_query = "SELECT * FROM PositionPlayers INNER JOIN Positions on PositionPlayers.Position_positionID = Positions.positionID WHERE Player_playerID=@id";
            SqlParameter[] check_params = new SqlParameter[1];
            check_params[0] = new SqlParameter("@id", id);
            List<Position> positions = db.Positions.SqlQuery(check_query, check_params).ToList();

            
            // Execute only if that player doesn't already play that position:
            if (positions.Count <= 0)
            {
                foreach (var position in positionArray)
                {
                    string position_insert = "INSERT INTO PositionPlayers (Position_PositionID, Player_PlayerID) VALUES (@position, @id)";
                    SqlParameter[] sqlparams = new SqlParameter[2];
                    sqlparams[0] = new SqlParameter("@id", id);
                    sqlparams[1] = new SqlParameter("@position", position);
                    db.Database.ExecuteSqlCommand(position_insert, sqlparams);
                }
            }

            // STEP 1: PULL DATA:
            Debug.WriteLine("Retrieving data of "+ FirstName + " " + LastName);

            string query = "UPDATE players SET firstname=@FirstName, lastname=@LastName, teamID=@teamID, hasPic=@hasPic, picExtension=@picExtension WHERE playerID=@id";
            SqlParameter[] parameters = new SqlParameter[6]; 

            Debug.WriteLine(query);

            parameters[0] = new SqlParameter("@FirstName", FirstName); // SECOND VALUE GETS THE VALUE FROM LINE 126
            parameters[1] = new SqlParameter("@LastName", LastName);
            parameters[2] = new SqlParameter("@teamID", teamID);
            parameters[3] = new SqlParameter("@id", id);
            parameters[4] = new SqlParameter("@hasPic", hasPic);
            parameters[5] = new SqlParameter("@picExtension", picExtension);

            Debug.WriteLine("Query: " + query);

            db.Database.ExecuteSqlCommand(query, parameters);

            return RedirectToAction("List");
        }

        public ActionResult DeleteConfirm(int id)
        {
            string query = "SELECT * FROM players WHERE playerID = @id";
            SqlParameter param = new SqlParameter("@id", id);
            Player selectedplayer = db.Players.SqlQuery(query, param).FirstOrDefault();

            return View(selectedplayer);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            string query = "DELETE FROM players WHERE playerID = @id";
            SqlParameter param = new SqlParameter("@id", id);
            db.Database.ExecuteSqlCommand(query, param);
            Debug.WriteLine("Delete Query: " + query);

            return RedirectToAction("List");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
