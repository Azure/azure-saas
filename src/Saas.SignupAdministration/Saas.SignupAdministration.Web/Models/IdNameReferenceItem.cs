#nullable disable

namespace Saas.SignupAdministration.Web.Models
{
    public record IdNameReferenceItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ReferenceData
    {
        public static List<IdNameReferenceItem> TenantCategories = new()
        {
            new IdNameReferenceItem { Id = 1, Name = SR.AutomotiveMobilityAndTransportationPrompt },
            new IdNameReferenceItem { Id = 2, Name = SR.EnergyAndSustainabilityPrompt },
            new IdNameReferenceItem { Id = 3, Name = SR.FinancialServicesPrompt },
            new IdNameReferenceItem { Id = 4, Name = SR.HealthcareAndLifeSciencesPrompt },
            new IdNameReferenceItem { Id = 5, Name = SR.ManufacturingAndSupplyChainPrompt },
            new IdNameReferenceItem { Id = 6, Name = SR.MediaAndCommunicationsPrompt },
            new IdNameReferenceItem { Id = 7, Name = SR.PublicSectorPrompt },
            new IdNameReferenceItem { Id = 8, Name = SR.RetailAndConsumerGoodsPrompt },
            new IdNameReferenceItem { Id = 9, Name = SR.SoftwarePrompt }
        };

        public static List<IdNameReferenceItem> ProductServicePlans = new()
        {
            new IdNameReferenceItem { Id = 5, Name = SR.FreePlan },
            new IdNameReferenceItem { Id = 6, Name = SR.BasicPlan },
            new IdNameReferenceItem { Id = 7, Name = SR.StandardPlan },
        };
    }
}
