using System.Collections.Generic;
using InsureGo_API.Models;

namespace InsureGo_API.Repository
{
    public interface IInsuranceRepository
    {
        // ---------- Account ----------
        User Register(User user);
        User Login(string email, string password);

        void ResetPassword(string email, string newPasswordHash);


        // ---------- Vehicle ----------
        Vehicle AddVehicle(Vehicle vehicle);
        List<Vehicle> GetVehicles();

        // ---------- Insurance ----------
        void BuyInsurance(Policy policy);

        // ---------- Policy ----------
        Policy GetPolicyById(int id);
        void RenewPolicy(string policyNumber);
        List<Policy> GetPoliciesByUser(int userId);

        // ---------- Premium ----------
        decimal CalculatePremium(int vehicleTypeId, int vehicleAge);

        // ---------- Payment ----------
        void MakePayment(Payment payment);

        // ---------- Claim ----------
        void RaiseClaim(Claim claim);

        // ----------------- Claim History -----------------
        List<ClaimViewModel> GetClaimHistory(string policyNumber);
        Policy GetPolicyByNumberAndMobile(string policyNumber, string mobile);

        Claim GetClaimById(int claimId);   
        void UpdateClaim(Claim claim);     
        int GetPendingClaimsCount();

        List<Claim> GetClaimsByStatus(string status);
    }
}
