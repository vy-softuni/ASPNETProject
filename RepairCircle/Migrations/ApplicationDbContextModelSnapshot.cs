using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RepairCircle.Data;

#nullable disable

namespace RepairCircle.Migrations;

[DbContext(typeof(ApplicationDbContext))]
partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=RepairCircleDb_Migrations_Working_20260420;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True")
            .Options;

        var context = new ApplicationDbContext(options);
        var method = typeof(ApplicationDbContext).GetMethod("OnModelCreating", BindingFlags.Instance | BindingFlags.NonPublic);
        method!.Invoke(context, new object[] { modelBuilder });
    }
}
