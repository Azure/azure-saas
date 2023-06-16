using System.ComponentModel.DataAnnotations;

namespace Saas.SignupAdministration.Web.Models;

public class BookingDto
{

    public long BookingId { get; set; }
    public string ExternalSchemeAdmin { get; set; } = string.Empty;

    public string CourseDate { get; set; } = string.Empty;

    public string BookingType { get; set; } = string.Empty;

    public string RetirementSchemeName { get; set; } = string.Empty;
    public string SchemePosition { get; set; } = string.Empty;
    public string TrainingVenue { get; set; } = string.Empty;
    public string PaymentMode { get; set; } = string.Empty;
    public string AdditionalRequirements { get; set; } = string.Empty;
    public long UserId { get; set; }

}
