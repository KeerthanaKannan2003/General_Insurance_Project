using InsureGo_API.Models;
using System.Linq;
using System.Web.Http;

namespace InsureGo_API.Api.Controllers
{
    [RoutePrefix("api/vehicle")]
    public class VehicleController : ApiController
    {
        InsureGoDBEntities db = new InsureGoDBEntities();

        // Add a new vehicle
        [HttpPost]
        [Route("add")]  // POST api/vehicle/add
        public IHttpActionResult AddVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
                return BadRequest("Invalid vehicle data");

            db.Vehicles.Add(vehicle);
            db.SaveChanges();

            return Ok(vehicle.VehicleId);
        }

        // Get all vehicles
        [HttpGet]
        [Route("all")]  // GET api/vehicle/all
        public IHttpActionResult GetVehicles()
        {
            var vehicles = db.Vehicles.ToList();
            return Ok(vehicles);
        }
    }
}
