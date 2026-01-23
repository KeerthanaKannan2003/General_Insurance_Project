using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InsureGo_API.Repository;
namespace InsureGo_API.Controllers
{

    [RoutePrefix("api/premium")]
    public class PremiumApiController : ApiController
    {
        IInsuranceRepository repo = new InsuranceRepository();

        [HttpGet]
        [Route("calculate/{vehicleTypeId}/{vehicleAge}")]
        public IHttpActionResult CalculatePremium(int vehicleTypeId, int vehicleAge)
        {
            return Ok(repo.CalculatePremium(vehicleTypeId, vehicleAge));
        }
    }

}
