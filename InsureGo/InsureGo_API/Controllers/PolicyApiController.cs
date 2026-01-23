using InsureGo_API.Models;
using System;
using System.Linq;
using System.Web.Http;
using InsureGo_API.Repository;
namespace InsureGo_API.Api.Controllers
{
    [RoutePrefix("api/insurance")]
    public class InsuranceApiController : ApiController
    {
        InsureGoDBEntities db = new InsureGoDBEntities();

        // Add Vehicle
        [HttpPost, Route("addvehicle")]
        public IHttpActionResult AddVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
                return BadRequest("Invalid vehicle data");

            db.Vehicles.Add(vehicle);
            db.SaveChanges();
            return Ok(vehicle.VehicleId);
        }

        // Create Policy (without generating PolicyNumber here)
        [HttpPost]
        [Route("createpolicy")]
        public IHttpActionResult CreatePolicy(Policy policy)
        {
            if (policy == null || policy.Duration == null)
                return BadRequest("Invalid policy data");

            policy.StartDate = DateTime.Now;
            policy.EndDate = DateTime.Now.AddYears((int)policy.Duration);
            policy.PolicyStatus = "Pending";

            db.Policies.Add(policy);
            db.SaveChanges();

            return Ok(policy.PolicyId);
        }


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

    }
}
