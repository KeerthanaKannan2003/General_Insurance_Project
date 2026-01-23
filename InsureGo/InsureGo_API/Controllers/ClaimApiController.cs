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


    [RoutePrefix("api/claim")]
    public class ClaimApiController : ApiController
    {
        IInsuranceRepository repo = new InsuranceRepository();

        [HttpPost]
        [Route("raise")]
        public IHttpActionResult RaiseClaim(Claim claim)
        {
            repo.RaiseClaim(claim);
            return Ok("Claim Raised");
        }

        [HttpGet]
        [Route("history/{policyId}")]
        public IHttpActionResult ClaimHistory(int policyId)
        {
            return Ok(repo.GetClaimHistory(policyId));
        }
    }

}
