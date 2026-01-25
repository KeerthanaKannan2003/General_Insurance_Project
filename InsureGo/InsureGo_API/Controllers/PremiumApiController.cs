using System.Web.Http;
using InsureGo_API.Repository;

namespace InsureGo_API.Controllers
{
    [RoutePrefix("api/premium")]
    public class PremiumApiController : ApiController
    {
        IInsuranceRepository repo = new InsuranceRepository();

        // GET: api/premium/calculate/2/5
        [HttpGet]
        [Route("calculate/{vehicleTypeId}/{vehicleAge}")]
        public IHttpActionResult CalculatePremium(int vehicleTypeId, int vehicleAge)
        {
            try
            {
                decimal premium = repo.CalculatePremium(vehicleTypeId, vehicleAge);
                return Ok(premium);
            }
            catch (System.Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
