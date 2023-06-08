using Saas.Admin.Service.Interfaces;

namespace Saas.Admin.Service.Model;

public class SadUserDto : ISadUserDto
{
    public long Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string FullNames { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string Telephone { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;
}
