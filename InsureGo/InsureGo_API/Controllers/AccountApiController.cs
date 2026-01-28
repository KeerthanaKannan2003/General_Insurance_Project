using InsureGo_API.Models;

using System;

using System.Linq;

using System.Security.Cryptography;

using System.Text;

using System.Web.Http;

using InsureGo_API.Repository;

namespace InsureGo_API.Api.Controllers

{

    [RoutePrefix("api/account")]

    public class AccountController : ApiController

    {

        InsureGoDBEntities db = new InsureGoDBEntities();

        private readonly IInsuranceRepository repo;

        public AccountController()

        {

            repo = new InsuranceRepository();

        }

        // REGISTER

        [HttpPost]

        [Route("register")]

        public IHttpActionResult Register(User user)

        {

            if (user == null)

                return BadRequest("Invalid user data");

            if (db.Users.Any(u => u.EmailId == user.EmailId))

                return BadRequest("Email already exists");

            user.CreatedDate = DateTime.Now;

            user.PasswordHash = HashPassword(user.PasswordHash); 

            repo.Register(user);

            return Ok("Registered Successfully");

        }

        // LOGIN

        [HttpPost]

        [Route("login")]

        public IHttpActionResult Login(string email, string password)

        {

            var user = db.Users.FirstOrDefault(u => u.EmailId == email);

            if (user == null)

                return Unauthorized(); 

            string hashedPassword = HashPassword(password);

            if (user.PasswordHash != hashedPassword)

                return Unauthorized(); 

            return Ok(new

            {

                user.UserId,

                user.FullName,

                user.EmailId

            });

        }

        // RESET PASSWORD

        [HttpPost]

        [Route("resetpassword")]

        public IHttpActionResult ResetPassword([FromBody] User model)

        {

            if (model == null || string.IsNullOrEmpty(model.EmailId) || string.IsNullOrEmpty(model.PasswordHash))

                return BadRequest("Invalid request");

            var user = db.Users.FirstOrDefault(u => u.EmailId == model.EmailId);

            if (user == null)

                return NotFound();

            user.PasswordHash = HashPassword(model.PasswordHash);

            db.SaveChanges();

            return Ok("Password reset successfully");

        }

        // ================= HASH FUNCTION =================

        private string HashPassword(string password)

        {

            using (var sha = SHA256.Create())

            {

                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

                return BitConverter.ToString(bytes).Replace("-", "");

            }

        }

    }

}

