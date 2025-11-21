using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CMCS_Web.Data;
using CMCS_Web.Models;

namespace CMCS_Web.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ApplicationDbContext _db;
        public ClaimService(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _db.Claims.AsNoTracking().ToListAsync();
        }

        public async Task<Claim?> GetByIdAsync(int id)
        {
            return await _db.Claims.FindAsync(id);
        }

        public async Task AddAsync(Claim claim)
        {
            await _db.Claims.AddAsync(claim);
            await _db.SaveChangesAsync();
        }

        public async Task CreateAsync(Claim claim)
        {
            await AddAsync(claim);
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var entity = await _db.Claims.FindAsync(id);
            if (entity == null) return;
            entity.Status = status;
            _db.Claims.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Claims.FindAsync(id);
            if (entity == null) return;
            _db.Claims.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
