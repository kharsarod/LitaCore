﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Main
    {
        public async Task Initialize()
        {
            try
            {
                using var context = new AppDbContext();
               // await context.Database.EnsureCreatedAsync();
               // await context.Database.MigrateAsync();

                Log.Information("Database initialized.");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}
