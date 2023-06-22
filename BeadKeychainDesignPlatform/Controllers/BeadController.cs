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
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44386/api/");
        }
        private JavaScriptSerializer jss = new JavaScriptSerializer();


        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }
        




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
            string url = "BeadData/ListBeads";
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
            DetailsBead ViewModels= new DetailsBead();

            string url = "BeadData/FindBead/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BeadDto specificBead = response.Content.ReadAsAsync<BeadDto>().Result;
            ViewModels.specificBead = specificBead;

            //show associated keychains with this specific bead
            url = "KeychainData/ListKeychainsForBead/"+id;
            response = client.GetAsync(url).Result;
            IEnumerable<KeychainDto> associatedKeychain= response.Content.ReadAsAsync<IEnumerable< KeychainDto >>().Result;
            ViewModels.associatedKeychain= associatedKeychain;

            //show aviliable keychains
            url = "KeychainData/ListKeychainsNotIncludeBead/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<KeychainDto> aviliableKeychain = response.Content.ReadAsAsync<IEnumerable<KeychainDto>>().Result;
            ViewModels.aviliableKeychain = aviliableKeychain;


            return View(ViewModels);
        }



        //POST: Bead/Associate/{beadid}
        [HttpPost]
        [Authorize]
        public ActionResult Associate(int id, int keychainid)
        {
            //get token
            GetApplicationCookie();
            //Debug.WriteLine("beadid: " + id + "keychainid: " + keychainid);

            //call api to aassociate the bead with keychain
            string url = "BeadData/AssociateBeadsWithKeychain/" + id + "/" + keychainid;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }




        //Get: Bead/UnAssociate/{beadid}?KeychainId={keychainid}
        [HttpGet]
        [Authorize]
        public ActionResult UnAssociate(int id, int keychainid)
        {
            GetApplicationCookie();
            //Debug.WriteLine("beadid: " + id + "keychainid: " + keychainid);

            //call api to aassociate the bead with keychain
            string url = "BeadData/RemoveBeadsWithKeychain/" + id + "/" + keychainid;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }




        /// <summary>
        /// The MVC5 view called New.cshtml has a form to collect data for creating a new bead
        /// </summary>
        /// <returns>Send collected data to Create Method</returns>
        /// GET: Bead/New
        [Authorize]
        public ActionResult New()
        {
            
            //information about all colours in the system.
            //GET api/BeadColour/List
            string url = "BeadColourData/ListBeadColours";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<BeadColourDto> beadcoloursOptions = response.Content.ReadAsAsync<IEnumerable<BeadColourDto>>().Result;

            return View(beadcoloursOptions);
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
        /// POST: Bead/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Bead bead)
        {
            //GetApplicationCookie();
            string url = "BeadData/AddBead";
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


        /// <summary>
        /// The MVC5 view called Edit.cshtml has a form to collect the updating data for a sepcific bead.
        /// </summary>
        /// <param name="id">The specific bead primary key</param>
        /// <returns>Send collected data to Update Method</returns>
        /// GET: Bead/Edit/{id}
        [Authorize]
        public ActionResult Edit(int id)
        {
            
            UpdateBead ViewModel = new UpdateBead();

            //existing bead information
            string url = "BeadData/FindBead/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BeadDto SelectedBead = response.Content.ReadAsAsync<BeadDto>().Result;
            ViewModel.SelectedBead = SelectedBead;

            //include all the bead colour to choose from when update the bead
            url = "BeadColourData/ListBeadColours";
            response = client.GetAsync(url).Result;
            IEnumerable<BeadColourDto> beadcoloursOptions = response.Content.ReadAsAsync<IEnumerable<BeadColourDto>>().Result;
            ViewModel.beadcoloursOptions = beadcoloursOptions;




            return View(ViewModel);

        }

        /// <summary>
        /// Update the information of a specific bead to the system using the API
        /// </summary>
        /// <param name="id">The specific bead primary key</param>
        /// <param name="bead">Bead class</param>
        /// curl -d @beadupdate.json -H "Content-type:application/json" https://localhost:44386/api/BeadData/UpdateBead/7
        /// <returns>
        /// Successful: redirect to List page
        /// Fail: redirect to Error page
        /// </returns>
        /// POST: Bead/Update/{id}
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Bead bead)
        {
            //GetApplicationCookie();
            string url = "BeadData/UpdateBead/" + id;
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

        /// <summary>
        /// The MVC5 view called DeleteConfirm.cshtml to confirm with user whether they want to delete the selected bead
        /// </summary>
        /// <param name="id">The selected bead primary key</param>
        /// <returns>send the confirm answer to Delete method</returns>
        /// GET: Bead/Delete/{id}
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "BeadData/FindBead/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BeadDto selectedBead = response.Content.ReadAsAsync<BeadDto>().Result;
            return View(selectedBead);
        }

        /// <summary>
        /// Delete the specific bead
        /// </summary>
        /// <param name="id">The selected bead primary key</param>
        /// <returns>        
        /// Successful: redirect to List page
        /// Fail: redirect to Error page
        /// </returns>
        /// POST: Bead/Delete/{id}
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            //GetApplicationCookie();
            string url = "BeadData/DeleteBead/" + id;

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
