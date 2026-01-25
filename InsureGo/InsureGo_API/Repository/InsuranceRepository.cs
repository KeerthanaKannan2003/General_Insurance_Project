using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            db.RenewPolicy(policyNumber); // Stored Procedure
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

        public List<ClaimHistory> GetClaimHistory(int policyId)
        {
            return db.ClaimHistories
                     .Where(c => c.PolicyId == policyId)
                     .ToList();
        }
        public Policy GetPolicyByNumberAndMobile(string policyNumber, string mobile)
        {
            return db.Policies.FirstOrDefault(p =>
                p.PolicyNumber == policyNumber &&
                p.User.MobileNumber == mobile
            );
        }

    }

}