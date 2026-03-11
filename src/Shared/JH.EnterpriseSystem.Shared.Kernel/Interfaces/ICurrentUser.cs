using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Shared.Kernel.Interfaces
{
    public interface ICurrentUser
    {
        string UserId { get; }
        string UserName { get; }
        bool IsAuthenticated { get; }
    }
}
