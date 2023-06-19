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
    public class KeychainController : Controller
    {

        //code factoring
        private static readonly HttpClient client;
        static KeychainController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44386/api/KeychainData/");
        }
        private JavaScriptSerializer jss = new JavaScriptSerializer();


        /// <summary>
        /// Accessing information from keychain api controller to get the list of all the keychains
        /// </summary>
        /// <returns>List of keychains</returns>
        /// url: https://localhost:44386/api/KeychainData/ListKeychains
        /// <example>
        /// GET: Keychain/List
        /// </example>
        public ActionResult List()
        {
            string url = "ListKeychains";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<KeychainDto> keychains = response.Content.ReadAsAsync<IEnumerable<KeychainDto>>().Result;

            return View(keychains);
        }

        /// <summary>
        /// Accessing information from bead api controller to get the information of a specific keychain
        /// </summary>
        /// <param name="id">Primary key of a specific keychain</param>
        /// <returns>The information of that specific keychain</returns>
        /// url: https://localhost:44386/api/KeychainData/FindKeychain/{id}
        ///<example>
        /// GET: Keychain/Details/{id}
        /// </example>
        public ActionResult Details(int id)
        {
            string url = "FindKeychain/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            KeychainDto specificKeychain = response.Content.ReadAsAsync<KeychainDto>().Result;
            return View(specificKeychain);
        }



        /// <summary>
        /// The MVC5 view called New.cshtml has a form to collect data for creating a new keychain
        /// </summary>
        /// <returns>Send collected data to Create Method</returns>
        /// GET: Keychain/Create
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Add a new keychain to the system using the API
        /// </summary>
        /// <param name="keychain">Keychain class</param>
        /// curl -d @keychain.json -H "Content-type:application/json" https://localhost:44386/api/KeychainData/AddKeychain    
        /// <returns>
        /// Successful: redirect to List page
        /// Fail: redirect to Error page
        /// </returns>
        /// POST: Keychain/Create
        [HttpPost]
        public ActionResult Create(Keychain keychain)
        {
            string url = "AddKeychain";
            string jsonpayload = jss.Serialize(keychain);

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
        /// GET: Keychain/Error
        public ActionResult Error()
        {
            return View();
        }


        /// <summary>
        /// The MVC5 view called Edit.cshtml has a form to collect the updating data for a sepcific keychain.
        /// </summary>
        /// <param name="id">The specific keychain primary key</param>
        /// <returns>Send collected data to Update Method</returns>
        /// GET: Keychain/Edit/{id}
        public ActionResult Edit(int id)
        {
            string url = "FindKeychain/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            KeychainDto specificKeychain = response.Content.ReadAsAsync<KeychainDto>().Result;
            return View(specificKeychain);
        }

        /// <summary>
        /// Update the information of a specific keychain to the system using the API
        /// </summary>
        /// <param name="id">The specific keychain primary key</param>
        /// <param name="bead">Keychain class</param>
        /// curl -d @keychainupdate.json -H "Content-type:application/json" https://localhost:44386/api/KeychainData/UpdateKeychain/3
        /// <returns>
        /// Successful: redirect to List page
        /// Fail: redirect to Error page
        /// </returns>
        /// POST: Keychain/Update/{id}
        [HttpPost]
        public ActionResult Update(int id, Keychain keychain)
        {
            string url = "UpdateKeychain/"+id;
            string jsonpayload = jss.Serialize(keychain);

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
        /// The MVC5 view called DeleteConfirm.cshtml to confirm with user whether they want to delete the selected keychain
        /// </summary>
        /// <param name="id">The selected keychain primary key</param>
        /// <returns>send the confirm answer to Delete method</returns>
        /// GET: Keychain/Delete/{id}
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FindKeychain/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            KeychainDto selectedKeychain = response.Content.ReadAsAsync<KeychainDto>().Result;
            return View(selectedKeychain);
        }



        /// <summary>
        /// Delete the specific keychain
        /// </summary>
        /// <param name="id">The selected keychain primary key</param>
        /// <returns>        
        /// Successful: redirect to List page
        /// Fail: redirect to Error page
        /// </returns>
        /// POST: Keychain/Delete/{id}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "DeleteKeychain/"+id;

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
