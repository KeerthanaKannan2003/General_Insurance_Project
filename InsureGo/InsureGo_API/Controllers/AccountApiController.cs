using InsureGo_API.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Http;

namespace InsureGo_API.Api.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        InsureGoDBEntities db = new InsureGoDBEntities();

        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register(User user)
        {
            if (db.Users.Any(u => u.EmailId == user.EmailId))
                return BadRequest("Email already exists");

            user.CreatedDate = DateTime.Now;
            db.Users.Add(user);
            db.SaveChanges();
            return Ok("Registered Successfully");
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(string email, string password)
        {
            var user = db.Users
                .FirstOrDefault(u => u.EmailId == email && u.PasswordHash == password);

            if (user == null)
                return Unauthorized();

            return Ok(user);
        }

    }

}
