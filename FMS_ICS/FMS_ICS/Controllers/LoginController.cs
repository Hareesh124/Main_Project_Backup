using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS_ICS.Models;

namespace FMS_ICS.Controllers
{

    public class LoginController : Controller
    {
        fms_db_icsEntities db = new fms_db_icsEntities();

        // GET: Login
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }

        // POST: User Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserLogin(string username, string password)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u =>
                    u.Username == username &&
                    u.Password == password 
                //    && u.Status == "Active"
                );

                if (user != null)
                {
                    // Implement session management
                    Session["UserID"] = user.UserID;
                    return View();
                    //return RedirectToAction("Dashboard", "User");
                }

                ModelState.AddModelError("", "Invalid login attempt");
            }
            catch
            {
                // Log error and handle exception
            }

            return View("Index");
        }
        
        // POST: Admin Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminLogin(string username, string password)
        {
            try
            {
                var admin = db.Admins.FirstOrDefault(a =>
                    a.Username == username &&
                    a.Password == password
                );

                if (admin != null)
                {
                    // Implement session management
                    Session["AdminID"] = admin.AdminID;
                    return View();
                    //return RedirectToAction("Dashboard", "Admin");
                }

                ModelState.AddModelError("", "Invalid login attempt");
            }
            catch
            {
                // Log error and handle exception
            }

            return View("Index");
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

    }
}