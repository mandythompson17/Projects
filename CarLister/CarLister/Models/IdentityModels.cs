using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CarLister.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("Car", throwIfV1Schema: false)
        {
        }

        public DbSet<Car> Cars { get; set; }

        public async Task<List<string>> GetYears()
        {
            return await this.Database
                .SqlQuery<string>("ModelYears").ToListAsync();
        }

        public async Task<List<string>> GetMakesFromYear(string year)
        {
            var yearParam = new SqlParameter("@year", year);

            return await this.Database
                .SqlQuery<string>("MakesByYear @year", yearParam).ToListAsync();
        }

        public async Task<List<string>> GetModelsFromYearMake(string year, string make)
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make_name", make);

            return await this.Database
                .SqlQuery<string>("ModelsByYearMake @year, @make_name", yearParam, makeParam).ToListAsync();
        }

        public async Task<List<string>> GetTrimsFromYearMakeModel(string year, string make, string model)
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make_name", make);
            var modelParam = new SqlParameter("@model", model);

            return await this.Database
                .SqlQuery<string>("TrimsByYearMakeModel @year, @make_name, @model", yearParam, makeParam, modelParam).ToListAsync();
        }

        public async Task<List<Car>> GetCarsFromYear(string year)
        {
            var yearParam = new SqlParameter("@year", year);

            return await this.Database
                .SqlQuery<Car>("CarsByYear @year", yearParam).ToListAsync();
        }

        public async Task<List<Car>> GetCarsFromYearMake(string year, string make)
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make_name", make);

            return await this.Database
                .SqlQuery<Car>("CarsByYearMake @year, @make_name", yearParam, makeParam).ToListAsync();
        }

        public async Task<List<Car>> GetCarsFromYearMakeModel(string year, string make, string model)
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make_name", make);
            var modelParam = new SqlParameter("@model", model);

            return await this.Database
                .SqlQuery<Car>("CarsByYearMakeModel @year, @make_name, @model", yearParam, makeParam, modelParam).ToListAsync();
        }

        public async Task<List<Car>> GetCarsFromYearMakeModelTrim(string year, string make, string model, string trim)
        {
            var yearParam = new SqlParameter("@year", year ?? "");
            var makeParam = new SqlParameter("@make_name", make ?? "");
            var modelParam = new SqlParameter("@model", model ?? "");
            var trimParam = new SqlParameter("@trim", trim ?? "" );

            return await this.Database
                .SqlQuery<Car>("CarsByYearMakeModelTrim @year, @make_name, @model, @trim", yearParam, makeParam, modelParam, trimParam).ToListAsync();
        }

        public async Task<List<Car>> GetCars(string year, string make, string model, string trim, string filter, bool? paging, int? page, int? perPage, string sortColumn, string sortDirection, bool? sortByReverse)
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make", make ?? "");
            var modelParam = new SqlParameter("@model", model ?? "");
            var trimParam = new SqlParameter("@trim", trim ?? "");
            var filterParam = new SqlParameter("@filter", filter ?? "");
            var pagingParam = new SqlParameter("@paging", paging ?? true);
            var pageParam = new SqlParameter("@page", page ?? 0);
            var perPageParam = new SqlParameter("@perPage", perPage ?? 10);
            var sortCParam = new SqlParameter("@sortColumn", sortColumn ?? "");
            var sortDParam = new SqlParameter("@sortDirection", sortDirection ?? "");

            if (sortByReverse == true)
            {
                sortDParam = new SqlParameter("@sortDirection", "DESC");
            }

            else
            {
                sortDParam = new SqlParameter("@sortDirection", "ASC");
            }

            return await this.Database
                .SqlQuery<Car>("GetCars @year, @make, @model, @trim, @filter, @paging, @page, @perPage, @sortColumn, @sortDirection",
                yearParam, makeParam, modelParam, trimParam, filterParam, pagingParam, pageParam, perPageParam, sortCParam, sortDParam).ToListAsync();
        }

        public async Task<int> GetCarCount(string year, string make , string model , string trim , string filter )
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make", make);
            var modelParam = new SqlParameter("@model", model);
            var trimParam = new SqlParameter("@trim", trim);
            var filterParam = new SqlParameter("@filter", filter ?? "");

            return await this.Database
                .SqlQuery<int>("CountCars @year, @make, @model, @trim, @filter", yearParam, makeParam, modelParam, trimParam, filterParam).FirstOrDefaultAsync();
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}