﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Branch
{
    public sealed record BranchSelectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
