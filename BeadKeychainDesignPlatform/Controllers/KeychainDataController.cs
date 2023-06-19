using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BeadKeychainDesignPlatform.Models;

namespace BeadKeychainDesignPlatform.Controllers
{
    public class KeychainDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        /// <summary>
        /// Returns all the keychains in the system.
        /// </summary>
        /// <returns>
        /// All the keychains in the system with the name of it
        /// </returns>
        /// <example>
        /// GET: api/KeychainData/ListKeychains
        /// </example>
        [HttpGet]
        [ResponseType(typeof(KeychainDto))]
        public IHttpActionResult ListKeychains()
        {
            List<Keychain> Keychains = db.Keychains.ToList();
            List<KeychainDto> KeychainDtos = new List<KeychainDto>();

            Keychains.ForEach(k => KeychainDtos.Add(new KeychainDto()
            {
                KeychainId = k.KeychainId,
                KeychainName = k.KeychainName
            }));

            return Ok(KeychainDtos);
        }


        /// <summary>
        /// Returns all the keychains contain a specific bead.
        /// </summary>
        /// <param name="id">Bead primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all keychains in the database that contain a particular bead
        /// </returns>
        /// <example>
        /// GET: api/KeychainData/ListKeychainsForBead/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(KeychainDto))]
        public IHttpActionResult ListKeychainsForBead(int id)
        {
            List<Keychain> Keychains = db.Keychains.Where(
                k=>k.Beads.Any(
                    b=>b.BeadId==id)
                ).ToList();
            List<KeychainDto> KeychainDtos = new List<KeychainDto>();

            Keychains.ForEach(k => KeychainDtos.Add(new KeychainDto()
            {
                KeychainId = k.KeychainId,
                KeychainName = k.KeychainName
            }));

            return Ok(KeychainDtos);
        }



        /// <summary>
        /// Returns all the keychains NOT contain a specific bead.
        /// </summary>
        /// <param name="id">Bead primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all keychains in the database that NOT contain a particular bead
        /// </returns>
        /// <example>
        /// GET: api/KeychainData/ListKeychainsNotIncludeBead/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(KeychainDto))]
        public IHttpActionResult ListKeychainsNotIncludeBead(int id)
        {
            List<Keychain> Keychains = db.Keychains.Where(
                k => !k.Beads.Any(
                    b => b.BeadId == id)
                ).ToList();
            List<KeychainDto> KeychainDtos = new List<KeychainDto>();

            Keychains.ForEach(k => KeychainDtos.Add(new KeychainDto()
            {
                KeychainId = k.KeychainId,
                KeychainName = k.KeychainName
            }));

            return Ok(KeychainDtos);
        }

        /// <summary>
        /// Return the properties of one specific chosen bead in the system
        /// </summary>
        /// <param name="id">The BeadId of that chosen bead</param>
        /// <returns>
        /// All the properties releated to that chosen bead, including the bead description, bead name, bead picture(name of the picture), bead colour and the colour property
        /// </returns>
        /// <example>
        /// GET: api/KeychainData/FindKeychain/3
        /// </example> 
        [HttpGet]
        [ResponseType(typeof(Keychain))]
        public IHttpActionResult FindKeychain(int id)
        {
            Keychain Keychain = db.Keychains.Find(id);
            KeychainDto KeychainDto = new KeychainDto()
            {
                KeychainId = Keychain.KeychainId,
                KeychainName = Keychain.KeychainName
            };

            if (Keychain == null)
            {
                return NotFound();
            }

            return Ok(KeychainDto);
        }


        /// <summary>
        /// Update a specific keychain in the system with POST Data input
        /// </summary>
        /// <param name="id">The primary key of the specific keychain</param>
        /// <param name="keychain">JSON FORM DATA of a keychain</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/KeychainData/UpdateKeychain/5
        /// </example>
        /// <example>
        /// CREATE: A keychainupdate.json file, with JSON FORM DATA that want to input into system
        ///        {
        ///             "KeychainId": 3,
        ///             "KeychainName": "dog"
        ///         }
        /// COMMAND WINDOW: curl -d @keychainupdate.json -H "Content-type:application/json" https://localhost:44386/api/KeychainData/UpdateKeychain/3
        /// </example>
        // PUT: api/KeychainData/UpdateKeychain/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateKeychain(int id, Keychain keychain)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != keychain.KeychainId)
            {
                return BadRequest();
            }

            db.Entry(keychain).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KeychainExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Add a keychain in the system
        /// </summary>
        /// <param name="keychain">JSON FORM DATA of a keychain</param>
        /// <returns>
        /// Header: 201(Created)
        /// Content: KeychainId, Keychain Data 
        /// or
        /// Header: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/KeychainData/AddKeychain
        /// </example>
        /// <example>
        /// CREATE: A keychain.json file, with JSON FORM DATA that want to input into system
        ///        {
        ///             "KeychainName": "Cat"
        ///         }
        /// COMMAND WINDOW: curl -d @keychain.json -H "Content-type:application/json" https://localhost:44386/api/KeychainData/AddKeychain      
        /// RETURN:{"KeychainId":3,"KeychainName":"Cat","Beads":null}
        /// </example>
        [ResponseType(typeof(Keychain))]
        public IHttpActionResult AddKeychain(Keychain keychain)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Keychains.Add(keychain);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = keychain.KeychainId }, keychain);
        }


        /// <summary>
        /// Delete one specific keychain from the system by it's ID
        /// </summary>
        /// <param name="id">The primary key of the keychain that you want to delete</param>
        /// <returns>
        /// Header: 200 (OK, if it deletes successfully)
        /// or
        /// Header: 404 (NOT FOUND, if it deletes NOT successfully)
        /// </returns>
        /// <example>
        /// Post: api/KeychainData/DeleteKeychain/5
        /// curl -d "" "https://localhost:44386/api/KeychainData/DeleteKeychain/5"
        /// Form data: (empty)
        /// </example>
        [HttpPost]
        [ResponseType(typeof(Keychain))]
        public IHttpActionResult DeleteKeychain(int id)
        {
            Keychain keychain = db.Keychains.Find(id);
            if (keychain == null)
            {
                return NotFound();
            }

            db.Keychains.Remove(keychain);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool KeychainExists(int id)
        {
            return db.Keychains.Count(e => e.KeychainId == id) > 0;
        }
    }
}