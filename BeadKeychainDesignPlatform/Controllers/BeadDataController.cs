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
using System.Diagnostics;

namespace BeadKeychainDesignPlatform.Controllers
{
    public class BeadDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all the beads in the system.
        /// </summary>
        /// <returns>
        /// All the beads in the system with the properties of beads and also the colour and colour property of the beads
        /// </returns>
        /// <example>
        /// GET: api/BeadData/ListBeads
        /// </example>
        [HttpGet]
        [ResponseType(typeof(BeadDto))]
        public IHttpActionResult ListBeads()
        {
            List<Bead> Beads = db.Beads.ToList();
            List<BeadDto> BeadDtos = new List<BeadDto>();

            Beads.ForEach(b => BeadDtos.Add(new BeadDto()
            {
                BeadId = b.BeadId,
                BeadDescription= b.BeadDescription,
                BeadName = b.BeadName,
                BeadPicture=b.BeadPicture,
                ColourName = b.BeadColour.ColourName,
                ColourProperty=b.BeadColour.ColourProperty
            }));

            return Ok(BeadDtos);
        }


        /// <summary>
        /// Return the properties of one specific chosen bead in the system
        /// </summary>
        /// <param name="id">The BeadId of that chosen bead</param>
        /// <returns>
        /// All the properties releated to that chosen bead, including the bead description, bead name, bead picture(name of the picture), bead colour and the colour property
        /// </returns>
        /// <example>
        /// GET: api/BeadData/FindBead/3
        /// </example> 
        [HttpGet]
        [ResponseType(typeof(Bead))]
        public IHttpActionResult FindBead(int id)
        {
            Bead Bead = db.Beads.Find(id);
            BeadDto BeadDto = new BeadDto()
            {
                BeadId = Bead.BeadId,
                BeadDescription= Bead.BeadDescription,
                BeadName= Bead.BeadName,
                BeadPicture= Bead.BeadPicture,
                ColourName= Bead.BeadColour.ColourName,
                ColourProperty = Bead.BeadColour.ColourProperty
            };

            if (Bead == null)
            {
                return NotFound();
            }

            return Ok(BeadDto);
        }


        /// <summary>
        /// Update a specific bead in the system with POST Data input
        /// </summary>
        /// <param name="id">The primary key of the specific bead</param>
        /// <param name="bead">JSON FORM DATA of a bead</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/BeadData/UpdateBead/5
        /// </example>
        /// <example>
        /// CREATE: A beadupdate.json file, with JSON FORM DATA that want to input into system
        ///        {
        ///             "BeadId": 7,
        ///             "BeadName": "Pink blue bead with cat",
        ///             "BeadDescription": "Pink bead with cat face and fishes around it",
        ///             "BeadPicture": " ",
        ///             "ColourId": 1
        ///         }
        /// COMMAND WINDOW: curl -d @beadupdate.json -H "Content-type:application/json" https://localhost:44386/api/BeadData/UpdateBead/7
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateBead(int id, Bead bead)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bead.BeadId)
            {
                return BadRequest();
            }

            db.Entry(bead).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeadExists(id))
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
        /// Add a bead in the system
        /// </summary>
        /// <param name="bead">JSON FORM DATA of a bead</param>
        /// <returns>
        /// Header: 201(Created)
        /// Content: BeadId, Bead Data 
        /// or
        /// Header: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/BeadData/AddBead
        /// </example>
        /// <example>
        /// CREATE: A bead.json file, with JSON FORM DATA that want to input into system
        ///        {
        ///             "BeadName": "Baby blue bead with cat",
        ///             "BeadDescription": "Baby blue bead with cat face and fishes around it",
        ///             "BeadPicture": " ",
        ///             "ColourId": 3
        ///         }
        /// COMMAND WINDOW: curl -d @bead.json -H "Content-type:application/json" https://localhost:44386/api/BeadData/AddBead        
        /// RETURN:{"BeadId":7,"BeadName":"Baby blue bead with cat","BeadDescription":"Baby blue bead with cat face and fishes around it","BeadPicture":" ","ColourId":3,"BeadColour":null,"Keychains":null}
        /// </example>
        [HttpPost]
        [ResponseType(typeof(Bead))]
        public IHttpActionResult AddBead(Bead bead)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Beads.Add(bead);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = bead.BeadId }, bead);
        }


        /// <summary>
        /// Delete one specific bead from the system by it's ID
        /// </summary>
        /// <param name="id">The primary key of the bead that you want to delete</param>
        /// <returns>
        /// Header: 200 (OK, if it deletes successfully)
        /// or
        /// Header: 404 (NOT FOUND, if it deletes NOT successfully)
        /// </returns>
        /// <example>
        /// Post: api/BeadData/DeleteBead/5
        /// curl -d "" "https://localhost:44386/api/BeadData/DeleteBead/5"
        /// Form data: (empty)
        /// </example>
        [HttpPost]
        [ResponseType(typeof(Bead))]
        public IHttpActionResult DeleteBead(int id)
        {
            Bead bead = db.Beads.Find(id);
            if (bead == null)
            {
                return NotFound();
            }

            db.Beads.Remove(bead);
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

        private bool BeadExists(int id)
        {
            return db.Beads.Count(e => e.BeadId == id) > 0;
        }
    }
}