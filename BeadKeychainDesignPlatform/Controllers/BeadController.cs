using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using BeadKeychainDesignPlatform.Models;
using System.Web.Script.Serialization;
using BeadKeychainDesignPlatform.Models.ViewModels;
using System.Diagnostics;

namespace BeadKeychainDesignPlatform.Controllers
{
    public class BeadController : Controller
    {
        //code factoring
        private static readonly HttpClient client;
        static BeadController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44386/api/BeadData/");
        }
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        /// <summary>
        /// Accessing information from bead api controller to get the list of all the beads
        /// </summary>
        /// <returns>List of beads</returns>
        /// url: https://localhost:44386/api/BeadData/ListBeads
        /// <example>
        /// GET: Bead/List
        /// </example>
        public ActionResult List()
        {
            string url = "ListBeads";
            HttpResponseMessage response = client.GetAsync(url).Result; 
            IEnumerable<BeadDto> beads = response.Content.ReadAsAsync<IEnumerable<BeadDto>>().Result;

            return View(beads);
        }

        /// <summary>
        /// Accessing information from bead api controller to get the information of a specific bead
        /// </summary>
        /// <param name="id">Primary key of a specific bead</param>
        /// <returns>The information of that specific bead</returns>
        /// url: https://localhost:44386/api/BeadData/FindBead/{id}
        ///<example>
        /// GET: Bead/Details/{id}
        /// </example>
        public ActionResult Details(int id)
        {
            string url = "FindBead/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BeadDto specificBead = response.Content.ReadAsAsync<BeadDto>().Result;
            return View(specificBead);
        }

        /// <summary>
        /// The MVC5 view called New.cshtml has a form to collect data for creating a new bead
        /// </summary>
        /// <returns>Send collected data to Create Method</returns>
        /// GET: Bead/New
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Add a new bead to the system using the API
        /// </summary>
        /// <param name="bead">Bead class</param>
        /// curl -d @bead.json -H "Content-type:application/json" https://localhost:44386/api/BeadData/AddBead   
        /// <returns>
        /// Successful: redirect to List page
        /// Fail: redirect to Error page
        /// </returns>
        // POST: Bead/Create
        [HttpPost]
        public ActionResult Create(Bead bead)
        {
            string url = "AddBead";
            string jsonpayload = jss.Serialize(bead);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url,content).Result;

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
        /// GET: Bead/Error
        public ActionResult Error()
        {
            return View();
        }

        // GET: Bead/Edit/5
        public ActionResult Edit(int id)
        {
            //UpdateBead ViewModel = new UpdateBead();

            //existing bead information
            string url = "FindBead/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BeadDto specificBead = response.Content.ReadAsAsync<BeadDto>().Result;
            return View(specificBead);

        }

        // POST: Bead/Update/5
        [HttpPost]
        public ActionResult Update(int id, Bead bead)
        {
            
            string url = "UpdateBead/" + id;
            string jsonpayload = jss.Serialize(bead);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Bead/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FindBead/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BeadDto selectedBead = response.Content.ReadAsAsync<BeadDto>().Result;
            return View(selectedBead);
        }

        // POST: Bead/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "DeleteBead/" + id;

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
