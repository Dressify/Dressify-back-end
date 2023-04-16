using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class AnswearDto
    {
        public int QuestionId { get; set; }
        public string VendorId { get; set; }
        public int ProductId { get; set; }
        public string? Answear { get; set; }

    }
}
