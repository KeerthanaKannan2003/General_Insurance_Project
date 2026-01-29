using InsureGo_API.Models;
using InsureGo_API.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace InsureGo_API.Controllers
{
    [RoutePrefix("api/claim")]
    public class ClaimApiController : ApiController
    {
        private readonly IInsuranceRepository repo = new InsuranceRepository();

        // RAISE CLAIM
        [HttpPost]
        [Route("raise")]
        public IHttpActionResult RaiseClaim([FromBody] dynamic data)
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
                ClaimStatus = "Pending",
                ClaimAmount = 0
            };

            repo.RaiseClaim(claim);
            return Ok("Claim Raised Successfully");
        }

        // CLAIM HISTORY
        [HttpGet]
        [Route("history/{policyNumber}")]
        public IHttpActionResult ClaimHistory(string policyNumber)
        {
            if (string.IsNullOrEmpty(policyNumber))
                return BadRequest("Policy number is required.");

            var history = repo.GetClaimHistory(policyNumber);

            return Ok(history ?? new List<ClaimViewModel>());
        }




    /*    // APPROVE CLAIM
        [HttpPost]
        [Route("approve")]
        public IHttpActionResult ApproveClaim([FromBody] dynamic data)
        {
            int claimId = data.ClaimId;
            decimal amount = data.Amount;

            var claim = repo.GetClaimById(claimId);
            if (claim == null)
                return NotFound();

            claim.ClaimAmount = amount;
            claim.ClaimStatus = "Approved";

            repo.UpdateClaim(claim);
            return Ok("Claim Approved Successfully");
        }

        // Pending Claims List (Admin) 
        [HttpGet]
        [Route("pending")]
        public IHttpActionResult GetPendingClaims()
        {
            var pendingClaims = repo.GetClaimsByStatus("Pending")
                                    .Select(c => new
                                    {
                                        c.ClaimId,
                                        c.PolicyId,
                                        PolicyNumber = c.Policy.PolicyNumber,
                                        c.ClaimDate,
                                        c.ClaimReason,
                                        c.ClaimStatus,
                                        c.ClaimAmount
                                    })
                                    .ToList();

            return Ok(pendingClaims);
        }

        // Pending Claims Count (Admin Dashboard) 
        [HttpGet]
        [Route("pendingcount")]
        public IHttpActionResult PendingCount()
        {
            int count = repo.GetPendingClaimsCount();
            return Ok(count);
        }*/
    }
}
