using Dressify.DataAccess.Dtos;
using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository.IRepository
{
    public interface ISuperAdminRepository : IRepository<SuperAdmin>
    {
        Task<SuperAdmin> AddSuperAdminAsync(SuperAdmin sAdmin, string password);
        Task<Admin> CreateAdminAsync(AddAdminDto adminDto);
    }
}
    