using CarLister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace CarLister.Controllers
{   
    /// <summary>
    /// The cs controller for the car-finder application
    /// </summary>
    [RoutePrefix("api/Cars")]
    
    public class CarController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
       /// <summary>
       /// A class for all of the car properties/parameters
       /// </summary>
        public class ControllerParams
        {
            /// <summary>
            /// year = model_year of car
            /// </summary>
            public string year { get; set; }
            /// <summary>
            /// make = make of car
            /// </summary>
            public string make { get; set; }
            /// <summary>
            /// model = model of car
            /// </summary>
            public string model { get; set; }
            /// <summary>
            /// trim = trim level of car
            /// </summary>
            public string trim { get; set; }
            /// <summary>
            /// filter = search text input
            /// </summary>
            public string filter { get; set; }
            /// <summary>
            /// paging = whether to use paging
            /// </summary>
            public bool paging { get; set; }
            /// <summary>
            /// page = the page of the car list
            /// </summary>
            public int page { get; set; }
            /// <summary>
            /// perPage = how many cars to display on each page of list
            /// </summary>
            public int perPage { get; set; }
            /// <summary>
            /// sortColumn = the column by which to sort returned cars
            /// </summary>
            public string sortColumn { get; set; }
            /// <summary>
            /// sortDirection = the direction to sort by; either ASC or DESC
            /// </summary>
            public string sortDirection { get; set; }
            /// <summary>
            /// sortByReverse = variable to translate sortDirection for trNgGrid
            /// </summary>
            public bool sortByReverse { get; set; }
        }
        /// <summary>
        /// class for the id property of the car
        /// </summary>
        public class IdParam
        {
            /// <summary>
            /// id property of the IdParam class
            /// </summary>
            public int id { get; set; }
        }

        // GET: GetCar
        /// <summary>
        /// Gets an individual car listing by id
        /// </summary>
        /// <param name="id">the id of the individual car</param>
        /// <returns>A car listing</returns>
        [Route("GetCarDetails")]
        [HttpPost]
        public async Task<IHttpActionResult> GetCar(IdParam id)
        {
            var car = db.Cars.Find(id.id);
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
                    recalls = JsonConvert.DeserializeObject( await response.Content.ReadAsStringAsync());
                }
                catch(Exception e)
                {
                    return InternalServerError(e);
                }
            }

            return Ok(new { car, imageUrl, recalls });
        }
      /// <summary>
      /// Gets a list of all years in the database
      /// </summary>
      /// <returns>A list of years</returns>
        [Route("GetYears")]
        public async Task<List<string>> GetYears()
        {
            return await db.GetYears();
        }
        /// <summary>
        /// Gets a list of car makes from the database based on year input
        /// </summary>
        /// <param name="selected">an object with selected properties from drop down, including the user-selected year</param>
        /// <returns>A list of makes</returns>
        [Route("GetMakes")]
        [HttpPost]
        public async Task<List<string>> GetMakesFromYear(ControllerParams selected)
        {
            return await db.GetMakesFromYear(selected.year);
        }
        /// <summary>
        /// Gets a list of car models from the database based on year and make input
        /// </summary>
        /// <param name="selected">an object with selected properties from drop down, including the user-selected year and make</param>
        /// <returns>A list of models</returns>
        [Route("GetModels")]
        [HttpPost]
        public async Task<List<string>> GetModelsFromYearMake(ControllerParams selected)
        {
            return await db.GetModelsFromYearMake(selected.year, selected.make);
        }
        /// <summary>
        /// Gets a list of car trim levels from the database based on year, make, and model input
        /// </summary>
        /// <param name="selected">an object with selected properties from drop downs, including the user-selected year, make, and model</param>
        /// <returns>A list of car trim levels</returns>
        [Route("GetTrims")]
        [HttpPost]
        public async Task<List<string>> GetTrimsByYearMakeModel(ControllerParams selected)
        {
            return await db.GetTrimsFromYearMakeModel(selected.year, selected.make, selected.model);
        }
        /// <summary>
        /// Gets a list of cars from database based on year input
        /// </summary>
        /// <param name="selected">an object with selected properties from drop downs, including the user-selected year</param>
        /// <returns>A list of cars</returns>
        [Route("GetCarsFromYear")]
        [HttpPost]
        public async Task<List<Car>> GetCarsFromYear(ControllerParams selected)
        {
            return await db.GetCarsFromYear(selected.year);
        }
        /// <summary>
        /// Gets a list of cars from database based on year and make input
        /// </summary>
        /// <param name="selected">an object with selected properties from drop downs, including the user-selected year and make</param>
        /// <returns>A list of cars</returns>
        [Route("GetCarsFromYearMake")]
        [HttpPost]
        public async Task<List<Car>> GetCarsFromYearMake(ControllerParams selected)
        {
            return await db.GetCarsFromYearMake(selected.year, selected.make);
        }
        /// <summary>
        /// Gets a list of cars from database based on year, make, and model input
        /// </summary>
        /// <param name="selected">an object with selected properties from drop downs, including the user-selected year, make, and model</param>
        /// <returns>A list of cars</returns>
        [Route("GetCarsFromYearMakeModel")]
        [HttpPost]
        public async Task<List<Car>> GetCarsFromYearMakeModel(ControllerParams selected)
        {
            return await db.GetCarsFromYearMakeModel(selected.year, selected.make, selected.model);
        }
        /// <summary>
        /// Gets a list of cars from database based on year, make, model, and trim input
        /// </summary>
        /// <param name="selected">an object with selected properties from drop downs, including the user-selected year, make, model, and trim</param>
        /// <returns>A list of cars</returns>
        [Route("GetCarsFromYearMakeModelTrim")]
        [HttpPost]
        public async Task<List<Car>> GetCarsFromYearMakeModelTrim(ControllerParams selected)
        {
           if (selected == null)
               selected = new ControllerParams() {
                   year = "2000"
               };
            var cars =  await db.GetCarsFromYearMakeModelTrim(selected.year, selected.make, selected.model, selected.trim);
            return cars;
        }
        /// <summary>
        /// Returns a count of the cars returned by the query
        /// </summary>
        /// <param name="selected">an object with selected properties from drop downs and javascript instantiation</param>
        /// <returns>An int/the number of cars returned by query</returns>
        [Route("GetCarCount")]
        [HttpPost]
        public async Task<int> GetCarCount(ControllerParams selected)
        {
            var carCount = await db.GetCarCount(selected.year, selected.make, selected.model, selected.trim, selected.filter);
            return carCount;
        }
        /// <summary>
        /// Gets a list of cars from database based user inputs
        /// </summary>
        /// <param name="selected">an object with selected properties from drop downs and javascript instantiation</param>
        /// <returns>A list of cars</returns>
        [Route("GetCars")]
        [HttpPost]
        public async Task<IHttpActionResult> GetCars(ControllerParams selected)
        {
         var cars = (await db.GetCars(selected.year, selected.make, selected.model, selected.trim, selected.filter, selected.paging, selected.page, selected.perPage, selected.sortColumn, selected.sortDirection, selected.sortByReverse));
         return Ok(cars);
        }
    }
}
