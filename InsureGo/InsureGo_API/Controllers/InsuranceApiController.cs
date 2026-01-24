using InsureGo_API.Models;
using System.Linq;
using System.Web.Http;

namespace InsureGo_API.Api.Controllers
{
    [RoutePrefix("api/insurance")]
    public class InsuranceController : ApiController
    {
        InsureGoDBEntities db = new InsureGoDBEntities();

        // Create a new insurance policy
        [HttpPost]
        [Route("createpolicy")]  // POST api/insurance/createpolicy
        public IHttpActionResult CreatePolicy(Policy policy)
        {
            if (policy == null)
                return BadRequest("Invalid policy data");

            policy.PolicyStatus = "Active";
            db.Policies.Add(policy);
            db.SaveChanges();

            return Ok(policy.PolicyId);
        }

        // Get all policies
        [HttpGet]
        [Route("all")]  // GET api/insurance/all
        public IHttpActionResult GetPolicies()
        {
            var policies = db.Policies.ToList();
            return Ok(policies);
        }
    }
}