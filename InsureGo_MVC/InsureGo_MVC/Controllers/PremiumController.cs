using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Mvc;
using InsureGo_MVC.Models.ViewModels;

namespace InsureGo_MVC.Controllers
{
    public class PremiumController : Controller
    {
        [HttpGet]
        public ActionResult Calculate()
        {
            return View(new PremiumViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Calculate(PremiumViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                // Example ADO.NET call to SP CalculatePremium
                using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["InsureGoDb"].ConnectionString))
                using (var cmd = new SqlCommand("CalculatePremium", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VehicleTypeId", model.VehicleTypeId);
                    cmd.Parameters.AddWithValue("@VehicleAge", model.VehicleAge);

                    await con.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    model.PremiumAmount = (result != null) ? Convert.ToDecimal(result) : 0;
                }

                TempData["Success"] = "Premium calculated successfully!";
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error calculating premium: " + ex.Message;
                return View(model);
            }
        }
    }
}