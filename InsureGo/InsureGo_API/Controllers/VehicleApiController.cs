using InsureGo_API.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace InsureGo_API.Api.Controllers
{
    [RoutePrefix("api/vehicle")]
    public class VehicleController : ApiController
    {
        InsureGoDBEntities db = new InsureGoDBEntities();

        [HttpPost]
        [Route("addvehicle")]
        public IHttpActionResult AddVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
                return BadRequest("Invalid vehicle");

            db.Vehicles.Add(vehicle);
            db.SaveChanges();

            return Ok(vehicle.VehicleId);
        }
    }
}
