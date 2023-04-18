﻿using Dressify.DataAccess.Repository.IRepository;
using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository
{
    internal class ProductReportRepository : Repository<ProductReport>, IProductReportRepository
    {
        public ProductReportRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
