﻿using KoiCareSystem.Data.Base;
using KoiCareSystem.Data.DBContext;
using KoiCareSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiCareSystem.Data.Repository
{
    public class PondRepository : GenericRepository<Pond>
    {
        public PondRepository()
        {

        }

        public PondRepository(ApplicationDbContext context) => _context = context;

    }
}
