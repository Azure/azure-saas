using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas.SignupAdministration.Web;
public interface IAdminClientSettings
{
    public string AdminServiceScopes { get; set; }
}
