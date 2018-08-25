using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;
using Microsoft.EntityFrameworkCore;

namespace TradingBot.Core
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Position> Positions { get; set; }

        public DbSet<BitfinexCandle> Candles { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=TradingBotDB;Trusted_Connection=True;");
        }
    }
}
