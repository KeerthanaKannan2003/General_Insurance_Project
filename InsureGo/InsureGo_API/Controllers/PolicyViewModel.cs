namespace InsureGo_API.Api.Controllers
{
    internal class PolicyViewModel
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; }
        public object VehicleModel { get; set; }
        public string RegistrationNumber { get; set; }
        public string PlanType { get; set; }
        public int? Duration { get; set; }
        public decimal? PremiumAmount { get; set; }
        public string PolicyStatus { get; set; }
    }
}