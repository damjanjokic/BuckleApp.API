using BuckleApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuckleApp.Data.Data.ConfigExtensions;

public static class QueryFilters
{
    public static void ConfigureQueryFilters(this ModelBuilder builder)
    {
        builder.Entity<User>().HasQueryFilter(x => !x.isDeleted);
        builder.Entity<Appointment>().HasQueryFilter(x => !x.IsDeleted);
    }
    
}