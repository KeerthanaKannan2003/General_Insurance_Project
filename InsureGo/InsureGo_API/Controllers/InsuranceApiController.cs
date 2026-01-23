using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InsureGo_API.Models;
using InsureGo_API.Repository;
namespace InsureGo_API.Controllers
{
 
    [RoutePrefix("api/insurance")]
    public class InsuranceApiController : ApiController
    {
        InsureGoDBEntities db = new InsureGoDBEntities();

        // Add vehicle
        [HttpPost, Route("addvehicle")]
        public IHttpActionResult AddVehicle(Vehicle vehicle)
        {
            db.Vehicles.Add(vehicle);
            db.SaveChanges();
            return Ok(vehicle.VehicleId);
        }

        // Create policy
        [HttpPost, Route("createpolicy")]
        public IHttpActionResult CreatePolicy(Policy policy)
        {
            policy.PolicyStatus = "Active";
            db.Policies.Add(policy);
            db.SaveChanges();
            return Ok(policy.PolicyId);
        }
    }

}
