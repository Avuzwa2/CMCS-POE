using System.Collections.Generic;
using System.Threading.Tasks;
using CMCS_Web.Models;

namespace CMCS_Web.Services
{
    public interface IClaimService
    {
        Task<IEnumerable<Claim>> GetAllAsync();
        Task<Claim?> GetByIdAsync(int id);

        // create a claim (controller expects CreateAsync)
        Task CreateAsync(Claim claim);

        // older name kept for compatibility
        Task AddAsync(Claim claim);

        Task UpdateStatusAsync(int id, string status);

        // optional: remove if not used
        Task DeleteAsync(int id);
    }
}
