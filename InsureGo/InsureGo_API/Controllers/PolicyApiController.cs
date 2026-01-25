using InsureGo_API.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace InsureGo_API.Api.Controllers
{
    [RoutePrefix("api/insurance")]
    public class PolicyApiController : ApiController
    {
        InsureGoDBEntities db = new InsureGoDBEntities();

        // =========================
        // ADD VEHICLE
        // =========================
        [HttpPost]
        [Route("addvehicle")]
        public IHttpActionResult AddVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
                return BadRequest("Invalid vehicle data");

            db.Vehicles.Add(vehicle);
            db.SaveChanges();

            return Ok(vehicle.VehicleId);
        }

        // =========================
        // POLICY NUMBER GENERATOR
        // =========================
        private string GeneratePolicyNumber()
        {
            string year = DateTime.Now.Year.ToString();

            int lastId = db.Policies
                           .OrderByDescending(p => p.PolicyId)
                           .Select(p => p.PolicyId)
                           .FirstOrDefault();

            return $"POL-{year}-{(lastId + 1).ToString("D6")}";
        }

        // =========================
        // CREATE POLICY
        // =========================
        [HttpPost]
        [Route("createpolicy")]
        public IHttpActionResult CreatePolicy(Policy policy)
        {
            if (policy == null)
                return BadRequest("Invalid policy data");

            var duration = db.PolicyDurations
                             .FirstOrDefault(d => d.DurationId == policy.DurationId);

            if (duration == null || duration.DurationYears == null)
                return BadRequest("Invalid duration");

            policy.StartDate = DateTime.Now;
            policy.EndDate = policy.StartDate.Value
                             .AddYears(duration.DurationYears.Value);

            policy.PolicyStatus = "Active";

            // ✅ SAFE policy number
            policy.PolicyNumber = GeneratePolicyNumber();

            db.Policies.Add(policy);
            db.SaveChanges();

            return Ok(new
            {
                policy.PolicyId,
                policy.PolicyNumber
            });
        }

        // =========================
        // GET USER POLICIES
        // =========================
        [HttpGet]
        [Route("userpolicies/{userId}")]
        public IHttpActionResult GetUserPolicies(int userId)
        {
            var policies =
                from p in db.Policies
                join v in db.Vehicles on p.VehicleId equals v.VehicleId into pv
                from v in pv.DefaultIfEmpty()
                join plan in db.InsurancePlans on p.PlanId equals plan.PlanId
                join dur in db.PolicyDurations on p.DurationId equals dur.DurationId
                where p.UserId == userId
                select new
                {
                    p.PolicyNumber,
                    VehicleModel = v.VehicleModel,
                    RegistrationNumber = v.RegistrationNumber,
                    PlanType = plan.PlanName,
                    Duration = dur.DurationYears,
                    p.PremiumAmount,
                    p.PolicyStatus
                };

            return Ok(policies.ToList());
        }

        // =========================
        // GET POLICY BY NUMBER (for Details page)
        // =========================
        [HttpGet]
        [Route("policy/{policyNumber}")]
        public IHttpActionResult GetPolicyByNumber(string policyNumber)
        {
            var policy = db.Policies
                .Where(p => p.PolicyNumber == policyNumber)
                .Select(p => new
                {
                    p.PolicyNumber,
                    VehicleModel = p.Vehicle.VehicleModel,
                    p.Vehicle.RegistrationNumber,
                    p.PremiumAmount,
                    p.PolicyStatus
                })
                .FirstOrDefault();

            if (policy == null)
                return NotFound();

            return Ok(policy);
        }
    }
}
