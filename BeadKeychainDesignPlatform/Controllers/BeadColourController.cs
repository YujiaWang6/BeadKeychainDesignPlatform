using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using BeadKeychainDesignPlatform.Models;
using System.Web.Script.Serialization;
using System.Diagnostics;


namespace BeadKeychainDesignPlatform.Controllers
{
    public class BeadColourController : Controller
    {
        //code factoring
        private static readonly HttpClient client;
        static BeadColourController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44386/api/BeadColourData/");
        }
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        /// <summary>
        /// Accessing information from beadcolour api controller to get the list of all the bead colours
        /// </summary>
        /// <returns>List of bead colours</returns>
        /// curl: https://localhost:44386/api/BeadColourData/ListBeadColours
        /// <example>
        /// GET: BeadColour/List
        /// </example>
        public ActionResult List()
        {
            string url = "ListBeadColours";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<BeadColourDto> beadcolours = response.Content.ReadAsAsync<IEnumerable<BeadColourDto>>().Result;

            return View(beadcolours);
        }


        /// <summary>
        /// Accessing information from beadColour api controller to get the information of a specific bead colour
        /// </summary>
        /// <param name="id">Primary key of a specific bead colour</param>
        /// <returns>The information of that specific bead colour</returns>
        /// curl: https://localhost:44386/api/BeadData/FindBead/{id}
        ///<example>
        /// GET: BeadColour/Details/{id}
        /// </example>
        public ActionResult Details(int id)
        {
            string url = "FindBeadColour/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BeadColourDto specificColour = response.Content.ReadAsAsync<BeadColourDto>().Result;
            return View(specificColour);
        }


        /// <summary>
        /// The MVC5 view called New.cshtml has a form to collect data for creating a new bead colour
        /// </summary>
        /// <returns>Send collected data to Create Method</returns>
        /// GET: BeadColour/New
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Add a new bead colour to the system using the API
        /// </summary>
        /// <param name="beadColour">BeadColour class</param>
        /// curl -d @beadcolour.json -H "Content-type:application/json" https://localhost:44386/api/BeadColourData/AddBeadColour  
        /// <returns>
        /// Successful: redirect to List page
        /// Fail: redirect to Error page
        /// </returns>
        /// POST: BeadColour/Create
        [HttpPost]
        public ActionResult Create(BeadColour beadColour)
        {


            string url = "AddBeadColour";
            string jsonpayload = jss.Serialize(beadColour);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        /// <summary>
        /// The MVC5 view called Error.cshtml will hold error message
        /// </summary>
        /// GET: BeadColour/Error
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// The MVC5 view called Edit.cshtml has a form to collect the updating data for a sepcific bead colour.
        /// </summary>
        /// <param name="id">The specific bead colour primary key</param>
        /// <returns>Send collected data to Update Method</returns>
        /// GET: BeadColour/Edit/{id}
        public ActionResult Edit(int id)
        {
            string url = "FindBeadColour/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BeadColourDto specificColour = response.Content.ReadAsAsync<BeadColourDto>().Result;
            return View(specificColour);
        }


        /// <summary>
        /// Update the information of a specific bead colour to the system using the API
        /// </summary>
        /// <param name="id">The specific bead colour primary key</param>
        /// <param name="beadColour">BeadColour class</param>
        /// curl -d @beadcolourupdate.json -H "Content-type:application/json" https://localhost:44386/api/BeadColourData/UpdateBeadColour/3
        /// <returns>
        /// Successful: redirect to List page
        /// Fail: redirect to Error page
        /// </returns>
        /// POST: BeadColour/Update/{id}
        [HttpPost]
        public ActionResult Update(int id, BeadColour beadColour)
        {

            string url = "UpdateBeadColour/"+id;
            string jsonpayload = jss.Serialize(beadColour);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }


        /// <summary>
        /// The MVC5 view called DeleteConfirm.cshtml to confirm with user whether they want to delete the selected bead colour
        /// </summary>
        /// <param name="id">The selected bead colour primary key</param>
        /// <returns>send the confirm answer to Delete method</returns>
        /// GET: BeadColour/Delete/{id}
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FindBeadColour/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BeadColourDto selectedBeadColour = response.Content.ReadAsAsync<BeadColourDto>().Result;
            return View(selectedBeadColour);
        }


        /// <summary>
        /// Delete the specific bead colour
        /// </summary>
        /// <param name="id">The selected bead colour primary key</param>
        /// <returns>        
        /// Successful: redirect to List page
        /// Fail: redirect to Error page
        /// </returns>
        /// POST: BeadColour/Delete/{id}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "DeleteBeadColour/" + id;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
