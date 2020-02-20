using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Net;
using PassionProject_SusanBoratynska.Data;
using PassionProject_SusanBoratynska.Models;
using PassionProject_SusanBoratynska.Models.ViewModels;
using System.Diagnostics;
using System.IO;

namespace PassionProject_SusanBoratynska.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult About()
        {

            return View();

        }

        public ActionResult Players()
        {
            ViewBag.Message = "Your application description page.";

            return Redirect("/Player/List");
        }

        public ActionResult Positions()
        {
            ViewBag.Message = "Your contact page.";

            return Redirect("/Position/List");
        }
    }
}