﻿using Dressify.DataAccess.Dtos;
using Dressify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails>
    {
       int OrdersQuantity(IEnumerable<OrderDetails> Details);
        void returnProductQuantity(int orderid);

    }
}