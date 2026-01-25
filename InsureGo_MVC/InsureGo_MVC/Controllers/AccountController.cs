
using InsureGo_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace InsureGo_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly string API_URL = "https://localhost:44365/api/account/"; // Adjust port

        // ===================== LOGIN =====================
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);

                // Send email and password as query string
                var response = client.PostAsync(
                    $"login?email={model.EmailId}&password={HashPassword(model.Password)}",
                    null).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    dynamic user = JsonConvert.DeserializeObject(json);

                    Session["UserId"] = user.UserId;
                    Session["UserName"] = user.FullName;

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Error = "Invalid Email or Password";
                return View(model);
            }
        }


        // ===================== REGISTER =====================
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);

                // Match API User model properties exactly
                var payload = new
                {
                    FullName = model.FullName,
                    EmailId = model.EmailId,
                    ContactNumber = model.ContactNumber,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    PasswordHash = HashPassword(model.Password)
                };

                var response = client.PostAsJsonAsync("register", payload).Result;

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Login");

                // Read API error message
                var errorMsg = response.Content.ReadAsStringAsync().Result;
                ViewBag.Error = errorMsg;
                return View(model);
            }
        }


        // ===================== LOGOUT =====================
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // ===================== FORGOT PASSWORD =====================
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Generate simple 4-digit captcha
            string captcha = new Random().Next(1000, 9999).ToString();
            Session["FP_Email"] = model.EmailId;
            Session["FP_Captcha"] = captcha;

            return RedirectToAction("ResetPassword");
        }

        // ===================== RESET PASSWORD =====================
        [HttpGet]
        public ActionResult ResetPassword()
        {
            ViewBag.Captcha = Session["FP_Captcha"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            ViewBag.Captcha = Session["FP_Captcha"];

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Captcha != Session["FP_Captcha"]?.ToString())
            {
                ViewBag.Error = "Invalid Captcha";
                return View(model);
            }

            // ✅ Demo success flow
            Session.Remove("FP_Email");
            Session.Remove("FP_Captcha");

            TempData["Success"] = "Password reset successful! Please login.";
            return RedirectToAction("Login");
        }

        // ===================== PASSWORD HASH =====================
        private string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }
    }
}
