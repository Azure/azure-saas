namespace Saas.Admin.Service.Interfaces;

public interface ISadUserDto
{
    string Company { get; set; }
    string Country { get; set; }
    string? Email { get; set; }
    string FullNames { get; set; }
    long Id { get; set; }
    string Industry { get; set; }
    string Telephone { get; set; }
    string UserName { get; set; }
}