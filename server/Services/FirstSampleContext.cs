using Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Services
{
    

    public class FirstSampleContext : DbContext
    {
        public FirstSampleContext() : base("FirstSampleContext")
        {

        }

        public DbSet<SensorUser> SensorUserSet { get; set; }

        public DbSet<Sensor> SensorSet { get; set; }

        public DbSet<SensorData> SensorDataSet { get; set; }
        public DbSet<ChallengeType> ChallengeTypeSet { get; set; }

        public DbSet<Challenge> ChallengeSet { get; set; }
    }
}