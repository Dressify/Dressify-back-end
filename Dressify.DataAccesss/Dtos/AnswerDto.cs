﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Dtos
{
    public class AnswerDto
    {
        public int QuestionId { get; set; }
        public int ProductId { get; set; }
        public string? Answer { get; set; }

    }
}
