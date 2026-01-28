
using InsureGo_API.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
namespace InsureGo_API.Api.Controllers
{
    [RoutePrefix("api/insurance")]
    public class PolicyApiController : ApiController
    {
        InsureGoDBEntities db = new InsureGoDBEntities();

        public PolicyApiController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        // ADD VEHICLE

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

        // GENERATE POLICY NUMBER

        private string GeneratePolicyNumber()
        {
            string dateStr = DateTime.Now.ToString("yyyyMMdd");

            int lastId = db.Policies
                           .OrderByDescending(p => p.PolicyId)
                           .Select(p => p.PolicyId)
                           .FirstOrDefault();

            return $"POL{dateStr}{(lastId + 1).ToString("D6")}";
        }


        // GET PLANS

        [HttpGet]
        [Route("plans")]
        public IHttpActionResult GetPlans()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var plans = db.InsurancePlans.ToList();
            return Ok(plans);
        }


        // CALCULATE PREMIUM

        [HttpGet]
        [Route("calculatepremium")]
        public IHttpActionResult CalculatePremium(int vehicleTypeId, int vehicleAge)
        {
            var result = db.CalculatePremium(vehicleTypeId, vehicleAge).FirstOrDefault();
            return Ok(result ?? 0);
        }

        [HttpPost]
        [Route("buy")]
        public IHttpActionResult BuyPolicy(dynamic data)
        {
            if (data == null)
                return BadRequest("Invalid policy data");

            try
            {
                int userId = (int)data.UserId;
                int insuranceTypeId = (int)data.InsuranceTypeId;
                int vehicleId = (int)data.VehicleId;
                int planId = (int)data.PlanId;
                int durationYears = (int)data.DurationId; 
                decimal premiumAmount = (decimal)data.PremiumAmount;

                var duration = db.PolicyDurations
                                 .FirstOrDefault(d => d.DurationYears == durationYears);

                if (duration == null)
                    return BadRequest("Invalid duration years: " + durationYears);

                Policy policy = new Policy
                {
                    UserId = userId,
                    InsuranceTypeId = insuranceTypeId,
                    VehicleId = vehicleId,
                    PlanId = planId,
                    DurationId = duration.DurationId, 
                    PremiumAmount = premiumAmount,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(durationYears),
                    PolicyStatus = "Pending"
                };

                policy.PolicyNumber = GeneratePolicyNumber();

                db.Policies.Add(policy);
                db.SaveChanges();

                return Ok(policy.PolicyId);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        // GET USER POLICIES

        [HttpGet]
        [Route("userpolicies/{userId}")]
        public IHttpActionResult GetUserPolicies(int userId)
        {
            var data = (
                from p in db.Policies
                join v in db.Vehicles on p.VehicleId equals v.VehicleId into pv
                from v in pv.DefaultIfEmpty()
                join plan in db.InsurancePlans on p.PlanId equals plan.PlanId into pp
                from plan in pp.DefaultIfEmpty()
                join dur in db.PolicyDurations on p.DurationId equals dur.DurationId into pd
                from dur in pd.DefaultIfEmpty()
                where p.UserId == userId
                select new
                {
                    p.PolicyId,
                    p.PolicyNumber,

                    Manufacturer = v.Manufacturer,
                    Model = v.Model,
                    v.RegistrationNumber,

                    PlanType = plan.PlanName,
                    Duration = dur.DurationYears,

                    p.PremiumAmount,
                    p.PolicyStatus
                }
            ).ToList(); 
            var policies = data.Select(x => new PolicyViewModel
            {
                PolicyId = x.PolicyId,
                PolicyNumber = x.PolicyNumber,

                VehicleModel =
        string.IsNullOrWhiteSpace(x.Manufacturer) &&
        string.IsNullOrWhiteSpace(x.Model)
            ? "N/A"
            : (x.Manufacturer + " " + x.Model).Trim(),

                RegistrationNumber = x.RegistrationNumber ?? "N/A",

                PlanType = x.PlanType ?? "General Insurance",
                Duration = x.Duration ?? 1,

                PremiumAmount = x.PremiumAmount ?? 0,
                PolicyStatus = x.PolicyStatus
            }).ToList();

            return Ok(policies);
        }


        // GET POLICY BY NUMBER (for Details page)

        [HttpGet]
        [Route("policybyid/{policyNumber}")]
        public IHttpActionResult GetPolicyByNumber(string policyNumber)
        {
            if (string.IsNullOrWhiteSpace(policyNumber))
                return BadRequest("Policy number required.");

            string normalized = policyNumber.Trim().ToUpper();

            var policy = db.Policies
                .Where(p => p.PolicyNumber.ToUpper() == normalized)
                .Select(p => new
                {
                    p.PolicyNumber,
                    VehicleModel = p.Vehicle != null ? p.Vehicle.Model : "N/A",
                    RegistrationNumber = p.Vehicle != null ? p.Vehicle.RegistrationNumber : "N/A",
                    p.PremiumAmount,
                    p.PolicyStatus
                })
                .FirstOrDefault();

            if (policy == null)
                return NotFound();

            return Ok(policy);
        }

        // GET RENEWAL

        [HttpGet]
        [Route("renewaldetails/{policyNumber}")]
        public IHttpActionResult GetRenewalDetails(string policyNumber)
        {
            var policy = db.Policies
                           .Include(p => p.Vehicle)
                           .Include(p => p.Vehicle.VehicleType)
                           .FirstOrDefault(p => p.PolicyNumber == policyNumber);

            if (policy == null) return NotFound();

            var v = policy.Vehicle;
            if (v == null) return BadRequest("Vehicle not found for policy");
            if (DateTime.Now < v.PurchaseDate.AddYears(1))
            {
                return BadRequest("Renewal is allowed only after 1 year from purchase date");
            }


            var renewalData = new
            {
                VehicleId = v.VehicleId,
                VehicleTypeId = v.VehicleTypeId,
                VehicleType = v.VehicleType != null ? v.VehicleType.VehicleTypeName : "N/A",
                Manufacturer = v.Manufacturer,
                VehicleModel = v.Model,
                DrivingLicence = v.DrivingLicenseNumber,
                PurchaseDate = v.PurchaseDate,
                RegistrationNumber = v.RegistrationNumber,
                EngineNumber = v.EngineNumber,
                ChassisNumber = v.ChassisNumber,
                UserId = policy.UserId
            };

            return Ok(renewalData);
        }
    }
}