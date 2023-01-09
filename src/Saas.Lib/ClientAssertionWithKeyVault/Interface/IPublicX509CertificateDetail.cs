using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAssertionWithKeyVault.Interface;
public interface IPublicX509CertificateDetail
{
    string Kid { get;}
    Uri Id { get;}
    string Name { get;}    
}
