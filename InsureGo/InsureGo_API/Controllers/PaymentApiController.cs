using InsureGo_API.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace InsureGo_API.Api.Controllers
{
    [RoutePrefix("api/payment")]
    public class PaymentController : ApiController
    {
        InsureGoDBEntities db = new InsureGoDBEntities();

        [HttpPost]
        [Route("makepayment")]
        public IHttpActionResult MakePayment(Payment payment)
        {
            payment.PaymentDate = DateTime.Now;
            payment.PaymentStatus = "Success";
            db.Payments.Add(payment);

            var policy = db.Policies.FirstOrDefault(p => p.PolicyId == payment.PolicyId);
            if (policy == null)
                return BadRequest("Policy not found");

            policy.PolicyNumber = "POL" + DateTime.Now.ToString("yyyyMMddHHmmss");
            policy.PolicyStatus = "Active";

            db.SaveChanges();

            return Ok(policy.PolicyNumber);
        }

    }
}
