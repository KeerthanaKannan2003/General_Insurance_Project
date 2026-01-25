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
        public IHttpActionResult RaiseClaim(dynamic data)
        {
            string policyNumber = data.PolicyNumber;
            string mobileNumber = data.MobileNumber;
            string claimReason = data.ClaimReason;

            var policy = repo.GetPolicyByNumberAndMobile(policyNumber, mobileNumber);

            if (policy == null)
                return BadRequest("Invalid Policy Details");

            Claim claim = new Claim
            {
                PolicyId = policy.PolicyId,
                ClaimReason = claimReason,
                ClaimDate = DateTime.Now,
                ClaimStatus = "Pending"
            };

            repo.RaiseClaim(claim);
            return Ok("Claim Raised Successfully");
        }


        [HttpGet]
        [Route("history/{policyId}")]
        public IHttpActionResult ClaimHistory(int policyId)
        {
            return Ok(repo.GetClaimHistory(policyId));
        }
    }

}
