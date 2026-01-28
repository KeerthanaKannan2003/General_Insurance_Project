using System;
using System.Collections.Generic;
using System.Linq;
using InsureGo_API.Models;

namespace InsureGo_API.Repository
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private InsureGoDBEntities db = new InsureGoDBEntities();

        // ---------- Account ----------
        public User Register(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }

        public User Login(string email, string password)
        {
            return db.Users.FirstOrDefault(u => u.EmailId == email && u.PasswordHash == password);
        }

        // ---------- Vehicle ----------
        public Vehicle AddVehicle(Vehicle vehicle)
        {
            db.Vehicles.Add(vehicle);
            db.SaveChanges();
            return vehicle;
        }

        public List<Vehicle> GetVehicles()
        {
            return db.Vehicles.ToList();
        }

        // ---------- Insurance ----------
        public void BuyInsurance(Policy policy)
        {

            string prefix = "POL";
            string random = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            policy.PolicyNumber = $"{prefix}{random}IND";

            policy.PolicyStatus = "Active";
            db.Policies.Add(policy);
            db.SaveChanges();
        }

        // ---------- Policy ----------
        public Policy GetPolicyById(int id)
        {
            return db.Policies.FirstOrDefault(p => p.PolicyId == id);
        }

        public List<Policy> GetPoliciesByUser(int userId)
        {
            return db.Policies.Where(p => p.UserId == userId).ToList();
        }

        public void RenewPolicy(string policyNumber)
        {
            db.RenewPolicy(policyNumber);
        }

        // ---------- Premium ----------
        public decimal CalculatePremium(int vehicleTypeId, int vehicleAge)
        {
            var result = db.CalculatePremium(vehicleTypeId, vehicleAge).FirstOrDefault();
            return result ?? 0;
        }

        // ---------- Payment ----------
        public void MakePayment(Payment payment)
        {
            payment.PaymentStatus = "Success";
            db.Payments.Add(payment);
            db.SaveChanges();
        }

        // ---------- Claim ----------
        public void RaiseClaim(Claim claim)
        {
            claim.ClaimStatus = "Pending";
            db.Claims.Add(claim);
            db.SaveChanges();
        }

        // Use PolicyNumber instead of PolicyId
        public List<ClaimViewModel> GetClaimHistory(string policyNumber)
        { 
            var claims = db.ClaimHistories
                           .Where(c => c.PolicyNumber == policyNumber)
                           .ToList(); 

            var claimViewModels = claims.Select(c => new ClaimViewModel
            {
                ClaimId = c.ClaimId,
                PolicyNumber = c.PolicyNumber,
                ClaimAmount = c.ClaimAmount,
                ClaimStatus = c.ClaimStatus,
                ClaimDate = c.ClaimDate  
            }).ToList();

            return claimViewModels;
        }


        public Policy GetPolicyByNumberAndMobile(string policyNumber, string mobile)
        {
            return db.Policies
                     .Include("User") 
                     .ToList()
                     .FirstOrDefault(p =>
                         p.PolicyNumber == policyNumber &&
                         p.User != null &&
                         p.User.ContactNumber == mobile); 
        }

        public Claim GetClaimById(int claimId)
        {
            return db.Claims.FirstOrDefault(c => c.ClaimId == claimId);
        }

        public void UpdateClaim(Claim claim)
        {
            db.Entry(claim).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public void ResetPassword(string email, string newPasswordHash)
        {
            var user = db.Users.FirstOrDefault(u => u.EmailId == email);
            if (user == null)
                throw new Exception("User not found");

            user.PasswordHash = newPasswordHash;
            db.SaveChanges();
        }
        public List<Claim> GetClaimsByStatus(string status)
        {
            return db.Claims
                     .Where(c => c.ClaimStatus == status)
                     .ToList();
        }

        public int GetPendingClaimsCount()
        {
            return db.Claims.Count(c => c.ClaimStatus == "Pending");
        }

    }
}
