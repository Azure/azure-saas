using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas.Shared.Options;
public class HashOptions
{
    public const string SectionName = "Hashes";

    public string? PasswordHash { get; init; }
}
