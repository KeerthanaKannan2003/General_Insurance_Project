using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsureGo_API.Models;
using System.Collections.Generic;
namespace InsureGo_API.Repository
{
    public interface IInsuranceRepository
    {
        // ---------- Account ----------
        User Register(User user);
        User Login(string email, string password);

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
        List<ClaimHistory> GetClaimHistory(int policyId);
    }

}
