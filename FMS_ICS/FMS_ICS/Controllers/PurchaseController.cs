using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using FMS_ICS.Models;
using System.Data.Entity;

namespace FMS_ICS.Controllers
{
    public class PurchaseController : Controller
    {
        fms_db_icsEntities db = new fms_db_icsEntities();

        // GET: Purchases
        public ActionResult Index()
        {
            var purchases = db.Purchases.Include(p => p.User).Include(p => p.Product).ToList();
            return View(purchases);
        }

        // GET: Purchases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var purchase = db.Purchases
                .Include(p => p.User)
                .Include(p => p.Product)
                .FirstOrDefault(p => p.PurchaseID == id);

            if (purchase == null)
                return HttpNotFound();

            return View(purchase);
        }

        // GET: Purchases/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                purchase.PurchaseDate = DateTime.Now;
                purchase.RemainingEMI = purchase.TotalAmount;
                purchase.MonthlyEMI = purchase.TotalAmount / purchase.TenureMonths;

                db.Purchases.Add(purchase);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", purchase.UserID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", purchase.ProductID);
            return View(purchase);
        }

        // GET: Purchases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var purchase = db.Purchases
                .Include(p => p.User)
                .Include(p => p.Product)
                .FirstOrDefault(p => p.PurchaseID == id);

            if (purchase == null)
                return HttpNotFound();

            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", purchase.UserID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", purchase.ProductID);
            return View(purchase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                purchase.PurchaseDate = DateTime.Now;
                purchase.MonthlyEMI = purchase.TotalAmount / purchase.TenureMonths;
                purchase.RemainingEMI = purchase.TotalAmount -
                    (db.Transactions
                        .Where(t => t.PurchaseID == purchase.PurchaseID)
                        .Sum(t => (decimal?)t.AmountPaid) ?? 0);

                db.Entry(purchase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", purchase.UserID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", purchase.ProductID);
            return View(purchase);
        }
    }
}