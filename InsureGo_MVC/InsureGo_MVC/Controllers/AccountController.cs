using InsureGo_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InsureGo_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly string API_URL = "https://localhost:44365/api/account/";

        // ---------- LOGIN ----------
        [HttpGet]
        public ActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);

                var response = await client.PostAsync(
                    $"login?email={model.EmailId}&password={model.Password}", null);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic user = JsonConvert.DeserializeObject(json);

                    Session["UserId"] = user.UserId;
                    Session["UserName"] = user.FullName;

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Error = "Invalid Email or Password";
                return View(model);
            }
        }

        // ---------- REGISTER ----------
        [HttpGet]
        public ActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);

                var payload = new
                {
                    FullName = model.FullName,
                    EmailId = model.EmailId,
                    ContactNumber = model.ContactNumber,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    PasswordHash = model.Password
                };

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("register", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Login");

                var errorMsg = await response.Content.ReadAsStringAsync();
                ViewBag.Error = errorMsg;
                return View(model);
            }
        }

        // ---------- LOGOUT ----------
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // ---------- RESET PASSWORD ----------
        [HttpGet]
        public ActionResult ForgotPassword() => View();

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string captcha = new Random().Next(1000, 9999).ToString();
            Session["FP_Email"] = model.EmailId;
            Session["FP_Captcha"] = captcha;

            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            ViewBag.Captcha = Session["FP_Captcha"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            ViewBag.Captcha = Session["FP_Captcha"];

            if (!ModelState.IsValid) return View(model);

            if (model.Captcha != Session["FP_Captcha"]?.ToString())
            {
                ViewBag.Error = "Invalid Captcha";
                return View(model);
            }

            string email = Session["FP_Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Session expired. Please try again.";
                return RedirectToAction("ForgotPassword");
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);

                var payload = new
                {
                    EmailId = email,
                    PasswordHash = model.NewPassword
                };

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("resetpassword", content);

                if (!response.IsSuccessStatusCode)
                {
                    var apiError = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = "Failed to reset password: " + apiError;
                    return View(model);
                }
            }

            Session.Remove("FP_Email");
            Session.Remove("FP_Captcha");

            TempData["Success"] = "Password reset successful! Please login with new password.";
            return RedirectToAction("Login");
        }
    }
}