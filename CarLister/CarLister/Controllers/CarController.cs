using CarLister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CarLister.Controllers
{   [RoutePrefix("api/Cars")]
    public class CarController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
       
        // GET: GetCar
        /// <summary>
        /// Gets an individual car listing by id
        /// </summary>
        /// <param name="id">the id of the individual car</param>
        /// <returns>A car listing</returns>
        [Route("GetCar")]
        public async Task<IHttpActionResult> GetCar(int id)
        {
            var car = db.Cars.Find(id);
            if (car == null)
                return await Task.FromResult(NotFound());

            HttpResponseMessage response;

            var client = new BingSearchContainer(new Uri("https://api.datamarket.azure.com/Bing/search/"));
            client.Credentials = new NetworkCredential("accountKey", "noAYmJqSxik7zXJptJdXC4M8uT7TGXMjuWSoSTnx2jc");
            var marketData = client.Composite(
                "image",
                car.model_year + " " + car.make + " " + car.model_name + " " + car.model_trim,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
                ).Execute();

            //var imageUrl = marketData?.FirstOrDefault()?.Image?.FirstOrDefault()?.MediaUrl;
            var result = marketData.FirstOrDefault();
            var image = result != null ? result.Image : null;
            var firstImage = image != null ? image.FirstOrDefault() : null;
            var imageUrl = firstImage != null ? firstImage.MediaUrl : null;

            dynamic recalls;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://www.nhtsa.gov/");

                try
                {
                    response =
                        await httpClient.GetAsync("webapi/api/Recalls/vehicle/modelyear/" + car.model_year + "/make/" + car.make + "/model/" + car.model_name + "?format=json");
                    recalls = await response.Content.ReadAsStringAsync();
                }
                catch(Exception e)
                {
                    return InternalServerError(e);
                }
            }

            return Ok(new { car, imageUrl, recalls });
        }

        [Route("GetYears")]
        public async Task<List<string>> GetYears()
        {
            return await db.GetYears();
        }

        [Route("GetMakesFromYear")]
        public async Task<List<string>> GetMakesFromYear(string year)
        {
            return await db.GetMakesFromYear(year);
        }

        [Route("GetModelsFromYearMake")]
        public async Task<List<string>> GetModelsFromYearMake(string year, string make)
        {
            return await db.GetModelsFromYearMake(year, make);
        }

        [Route("GetTrimsByYearMakeModel")]
        public async Task<List<string>> GetTrimsByYearMakeModel(string year, string make, string model)
        {
            return await db.GetTrimsFromYearMakeModel(year, make, model);
        }
        [Route("GetCarsFromYear")]
        public async Task<List<Car>> GetCarsFromYear(string year)
        {
            return await db.GetCarsFromYear(year);
        }

        [Route("GetCarsFromYearMake")]
        public async Task<List<Car>> GetCarsFromYearMake(string year, string make)
        {
            return await db.GetCarsFromYearMake(year, make);
        }

        [Route("GetCarsFromYearMakeModel")]
        public async Task<List<Car>> GetCarsFromYearMakeModel(string year, string make, string model)
        {
            return await db.GetCarsFromYearMakeModel(year, make, model);
        }

        [Route("GetCarsFromYearMakeModelTrim")]
        public async Task<List<Car>> GetCarsFromYearMake(string year, string make, string model, string trim)
        {
            return await db.GetCarsFromYearMakeModelTrim(year, make, model, trim);
        }
    }
}
