using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using BeadKeychainDesignPlatform.Models;
using System.Web.Script.Serialization;
using System.Diagnostics;
using BeadKeychainDesignPlatform.Models.ViewModels;


namespace BeadKeychainDesignPlatform.Controllers
{
    public class BeadColourController : Controller
    {
        //code factoring
        private static readonly HttpClient client;
        static BeadColourController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44386/api/");
        }
        private JavaScriptSerializer jss = new JavaScriptSerializer();



        private void GetApplicationCookie()
        {
            string token = "";
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }



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
            string url = "BeadColourData/ListBeadColours";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<BeadColourDto> beadcolours = response.Content.ReadAsAsync<IEnumerable<BeadColourDto>>().Result;

            return View(beadcolours);
        }


        /// <summary>
        /// Accessing information from beadColour api controller to get the information of a specific bead colour
        /// </summary>
        /// <param name="id">Primary key of a specific bead colour</param>
        /// <returns>The information of that specific bead colour</returns>
        /// curl: https://localhost:44386/api/BeadColourData/FindBeadColour/{id}
        ///<example>
        /// GET: BeadColour/Details/{id}
        /// </example>
        public ActionResult Details(int id)
        {
            DetailsBeadColour ViewModels = new DetailsBeadColour();

            string url = "BeadColourData/FindBeadColour/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BeadColourDto specificColour = response.Content.ReadAsAsync<BeadColourDto>().Result;
            ViewModels.specificColour = specificColour;

            //showcase information about beads related to this colour
            //Send a request to gather information about beads related to a particular colour ID
            url = "BeadData/ListBeadsForColour/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<BeadDto> relatedBeads = response.Content.ReadAsAsync<IEnumerable<BeadDto>>().Result;
            ViewModels.relatedBeads = relatedBeads;

            return View(ViewModels);
        }


        /// <summary>
        /// The MVC5 view called New.cshtml has a form to collect data for creating a new bead colour
        /// </summary>
        /// <returns>Send collected data to Create Method</returns>
        /// GET: BeadColour/New
        [Authorize]
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
        [Authorize]
        public ActionResult Create(BeadColour beadColour)
        {

            GetApplicationCookie();
            string url = "BeadColourData/AddBeadColour";
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
        [Authorize]
        public ActionResult Edit(int id)
        {
            string url = "BeadColourData/FindBeadColour/" + id;
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
        [Authorize]
        public ActionResult Update(int id, BeadColour beadColour)
        {
            GetApplicationCookie();
            string url = "BeadColourData/UpdateBeadColour/" + id;
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
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "BeadColourData/FindBeadColour/" + id;
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
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "BeadColourData/DeleteBeadColour/" + id;

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
