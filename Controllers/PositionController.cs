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
    public class PositionController : Controller
    {
        private VolleyballContext db = new VolleyballContext();

        // Get Positions
        public ActionResult Index()
        {
            return View();
        }

        // List all Positions
        public ActionResult List(string searchkey)
        {
            Debug.WriteLine("Query: " + searchkey);
            // SQL query: List of Positions
            List<Position> myposition = db.Positions.SqlQuery("SELECT * FROM positions").ToList();

            if (searchkey != "")
            {
                myposition = db.Positions.SqlQuery("SELECT * FROM positions WHERE position LIKE '%" + searchkey + "%'").ToList();
            }

            return View(myposition);
        }

        public ActionResult Add()
        {
            return View();
        }

        // Add new Positions
        [HttpPost]
        public ActionResult Add(string PositionName)
        {
            string query = "INSERT INTO positions (position) values (@PositionName)";
            var parameter = new SqlParameter("@PositionName", PositionName);
            Debug.WriteLine("Query: " + query);

            db.Database.ExecuteSqlCommand(query, parameter);
            return RedirectToAction("List");
        }

        // Show single Position:
        public ActionResult Show(int id)
        {
            string query = "SELECT * FROM positions WHERE positionID = @id";
            var parameter = new SqlParameter("@id", id);
            Debug.WriteLine("Query: " + query);

            Position selectedposition = db.Positions.SqlQuery(query, parameter).FirstOrDefault();

            return View(selectedposition);
        }

        public ActionResult Update(int id)
        {
            string query = "SELECT * FROM positions WHERE positionID = @id";
            var parameter = new SqlParameter("@id", id);
            Debug.WriteLine("Query: " + query);

            Position selectedposition = db.Positions.SqlQuery(query, parameter).FirstOrDefault();

            return View(selectedposition);
        }

        // Update existing Position
        [HttpPost]
        public ActionResult Update(int id, string PositionName)
        {
            string query = "UPDATE positions SET position = @PositionName WHERE positionID = @id";
            SqlParameter[] sqlparams = new SqlParameter[2];
            sqlparams[0] = new SqlParameter("@PositionName", PositionName);
            sqlparams[1] = new SqlParameter("@id", id);
            db.Database.ExecuteSqlCommand(query, sqlparams);

            Debug.WriteLine("Query: " + query);

            return RedirectToAction("List");
        }

        public ActionResult DeleteConfirm(int id)
        {
            string query = "SELECT * FROM positions WHERE posidtionID=@id";
            SqlParameter param = new SqlParameter("@id", id);
            Position selectedposition = db.Positions.SqlQuery(query, param).FirstOrDefault();

            Debug.WriteLine("Query: " + query);

            return View(selectedposition);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            string query = "DELETE FROM positions WHERE positionID=@id";
            SqlParameter param = new SqlParameter("@id", id);
            db.Database.ExecuteSqlCommand(query, param);

            Debug.WriteLine("Query: " + query);

            // Remove this position from Players who have this position
            string refquery = "UPDATE position SET positionID = '' WHERE positionID=@id";
            db.Database.ExecuteSqlCommand(refquery, param);

            Debug.WriteLine("Reference Query: " + refquery);

            return RedirectToAction("List");

        }
    }
}