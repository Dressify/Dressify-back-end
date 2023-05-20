using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    public class PenaltyRepository : Repository<Penalty> , IPenaltyRepository 
    {
        private readonly ApplicationDbContext _context;
        public PenaltyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
