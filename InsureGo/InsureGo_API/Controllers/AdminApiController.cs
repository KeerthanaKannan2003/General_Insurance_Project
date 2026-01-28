using InsureGo_API.Models;
using InsureGo_API.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Web.Http;

namespace InsureGo_API.Api.Controllers
{
    [RoutePrefix("api/admin")]
    public class AdminApiController : ApiController
    {
        InsureGoDBEntities db = new InsureGoDBEntities();
        private readonly IInsuranceRepository repo;

        public AdminApiController()
        {
            repo = new InsuranceRepository();
        }

         // PENDING CLAIMS

        [HttpGet]
        [Route("claims")]
        public IHttpActionResult GetPendingClaims()
        {
            var claims = repo.GetClaimsByStatus("Pending")
                             .Select(c => new
                             {
                                 c.ClaimId,
                                 c.PolicyId,
                                 c.ClaimDate,
                                 c.ClaimReason,
                                 c.ClaimStatus,
                                 c.ClaimAmount,
                                 PolicyNumber = c.Policy.PolicyNumber 
                             })
                             .ToList();

            return Ok(claims);
        }


        // APPROVE CLAIM

        [HttpPost]
        [Route("setclaimamount")]
        public IHttpActionResult ApproveClaim([FromBody] JObject data)
        {
            if (data == null || data["ClaimId"] == null || data["Amount"] == null)
                return BadRequest("Invalid request data.");

            int claimId = data["ClaimId"].ToObject<int>();
            decimal claimAmount = data["Amount"].ToObject<decimal>();

            var claim = repo.GetClaimById(claimId);
            if (claim == null)
                return NotFound();

            claim.ClaimAmount = claimAmount;
            claim.ClaimStatus = "Approved";

            if (claim.Policy != null)
            {
                claim.Policy.PolicyStatus = "Active";
            }

            try
            {
                repo.UpdateClaim(claim);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok(new
            {
                Message = "Claim approved successfully",
                ClaimId = claim.ClaimId,
                ClaimAmount = claim.ClaimAmount,
                ClaimStatus = claim.ClaimStatus,
                PolicyNumber = claim.Policy.PolicyNumber
            });
        }

      
        [HttpGet]
        [Route("pendingcount")]
        public IHttpActionResult GetPendingClaimsCount()
        {
            try
            {
                int count = repo.GetPendingClaimsCount();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}