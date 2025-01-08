using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS_ICS.Models;

namespace FMS_ICS.Controllers
{
    public class DashboardController : Controller
    {
        fms_db_icsEntities db = new fms_db_icsEntities();

        // GET: Dashboard
        

            // GET: Dashboard
            public ActionResult Index()
            {
                var userId = 3; // Assuming user ID is 1, replace this with actual logic for logged-in user
                var user = db.Users.FirstOrDefault(u => u.UserID == userId);

                if (user == null)
                    return HttpNotFound("User not found.");

                var userCard = db.UserCards
                                       .Where(uc => uc.UserID == userId && uc.Status == "Active")
                                       .FirstOrDefault();

            var productsPurchased = db.Purchases
                                            .Where(p => p.UserID == userId)
                                            .OrderByDescending(p => p.PurchaseDate)
                                            .Take(1) // Displaying the last 5 products purchased
                                            .Select(p => new
                                            {
                                                p.PurchaseID,
                                                p.ProductID,
                                                p.PurchaseDate,
                                                p.TotalAmount,
                                                p.TenureMonths,
                                                p.MonthlyEMI,
                                                p.RemainingEMI,
                                                ProductName = db.Products.FirstOrDefault(pr => pr.ProductID == p.ProductID).ProductName // Joining Products table to fetch ProductName
                                            })
                                            .ToList();

            // Step 1: Retrieve a list of purchases for the user
            var productPurchases = db.Purchases
                                           .Where(p => p.UserID == userId)
                                           .OrderByDescending(p => p.PurchaseDate)
                                           .Take(1) // Displaying the last 5 products purchased
                                           .Select(p => p.PurchaseID) // Select only the PurchaseID
                                           .ToList(); // Execute the query and get the list of PurchaseIDs

            // Step 2: Retrieve the last transaction for the first purchase
            var lastTransaction = productPurchases.Any()
                ? db.Transactions
                          .Where(t => productPurchases.Contains(t.PurchaseID)) // Use the list of PurchaseIDs
                          .OrderByDescending(t => t.TransactionDate)
                          .FirstOrDefault()
                : null;

            // Get the remaining balance on the user's card
            var remainingBalance = userCard?.RemainingLimit ?? 0;

                // Passing the data directly to the view
                ViewBag.UserName = user.FullName;
                ViewBag.CardNumber = userCard?.CardNumber;
                ViewBag.Validity = userCard?.Validity;
                ViewBag.RemainingBalance = remainingBalance;
                ViewBag.ProductsPurchased = productsPurchased;
                ViewBag.LastTransaction = lastTransaction;

                return View();
            }
        }
        //    public ActionResult Index()
        //    {
        //        var userId = 1; // Assuming user ID is 1, replace this with actual logic for logged-in user
        //        var user = db.Users.FirstOrDefault(u => u.UserID == userId);

        //        if (user == null)
        //            return HttpNotFound("User not found.");

        //        var userCard = db.UserCards
        //                               .Where(uc => uc.UserID == userId && uc.Status == "Active")
        //                               .FirstOrDefault();

        //        var productsPurchased = db.Purchases
        //                                        .Where(p => p.UserID == userId)
        //                                        .OrderByDescending(p => p.PurchaseDate)
        //                                        .Take(5) // Displaying the last 5 products purchased
        //                                        .ToList();

        //        //var lastTransaction = db.Transactions
        //        //                              .Where(t => t.PurchaseID == productsPurchased.FirstOrDefault()?.PurchaseID)
        //        //                              .OrderByDescending(t => t.TransactionDate)
        //        //                              .FirstOrDefault();

        //        // Get the remaining balance on the user's card
        //        var remainingBalance = userCard?.RemainingLimit ?? 0;

        //        // Passing the data directly to the view
        //        ViewBag.UserName = user.FullName;
        //        ViewBag.CardNumber = userCard?.CardNumber;
        //        ViewBag.Validity = userCard?.Validity;
        //        ViewBag.RemainingBalance = remainingBalance;
        //        //ViewBag.ProductsPurchased = productsPurchased;
        //        //ViewBag.LastTransaction = lastTransaction;

        //        return View();
        //    }
        //}

}