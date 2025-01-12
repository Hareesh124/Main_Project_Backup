using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS_ICS.Models;

namespace FMS_ICS.Controllers
{
    public class RegisterController : Controller
    {
        public string GenerateCardNumber()
        {
            // Use a random generator to ensure randomness
            Random random = new Random();

            // Generate a 16-digit card number
            string cardNumber = string.Concat(Enumerable.Range(0, 16).Select(_ => random.Next(0, 10).ToString()));

            // Check if the card number is unique in the database
            while (_db.UserCards.Any(c => c.CardNumber == cardNumber))
            {
                cardNumber = string.Concat(Enumerable.Range(0, 16).Select(_ => random.Next(0, 10).ToString()));
            }

            return cardNumber;
        }
        fms_db_icsEntities _db = new fms_db_icsEntities();


        public ActionResult Index()
        {

            var cardTypes = _db.CardTypes.ToList();

            ViewBag.CardTypes = new SelectList(cardTypes, "CardTypeID", "CardType1");

            return View();

        }


        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user, int selectedCardType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check eligibility for the selected card type
                    var cardType = _db.CardTypes.FirstOrDefault(c => c.CardTypeID == selectedCardType);
                    if (cardType == null)
                    {
                        ModelState.AddModelError("selectedCardType", "Invalid card type selected.");
                        throw new Exception("Invalid card type.");
                    }

                    // Save user details in the database
                    user.RegistrationDate = DateTime.Now;
                    user.Status = "Pending";
                    _db.Users.Add(user);
                    _db.SaveChanges();

                    // Create user card entry
                    var userCard = new UserCard
                    {
                        UserID = user.UserID,
                        CardTypeID = selectedCardType,
                        CardNumber = GenerateCardNumber(),
                        RemainingLimit = cardType.LimitAmount,
                        Validity = DateTime.Now.AddYears(5), // Set validity period
                        Status = "Inactive"
                    };
                    _db.UserCards.Add(userCard);
                    _db.SaveChanges();

                    // Create document verification entry
                    var documentVerification = new DocumentVerification
                    {
                        UserID = user.UserID,
                        DocumentType = "KYC",
                        DocumentStatus = "Pending",
                        Remarks = "Awaiting Verification"
                    };
                    _db.DocumentVerifications.Add(documentVerification);
                    _db.SaveChanges();

                    // Redirect to User Dashboard
                    //return RedirectToAction("Success");
                }
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                System.Diagnostics.Debug.WriteLine("Error in registration: " + ex.Message);
            }

            ViewBag.CardTypes = new SelectList(_db.CardTypes, "CardTypeID", "CardType1");
            return View(user);
        }

    }
}