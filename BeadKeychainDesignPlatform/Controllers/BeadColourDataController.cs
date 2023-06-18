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
    public class BeadColourDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        /// <summary>
        /// Returns all the bead colours in the system.
        /// </summary>
        /// <returns>
        /// All the bead colours in the system with the colour name and colour property
        /// </returns>
        /// <example>
        /// GET: api/BeadColourData/ListBeadColours
        /// </example>
        [HttpGet]
        [ResponseType(typeof(BeadColourDto))]
        public IHttpActionResult ListBeadColours()
        {
            List<BeadColour> BeadColours = db.BeadsColours.ToList();
            List<BeadColourDto> BeadColoursDtos = new List<BeadColourDto>();

            BeadColours.ForEach(c => BeadColoursDtos.Add(new BeadColourDto()
            {
                ColourId = c.ColourId,
                ColourName = c.ColourName,
                ColourProperty = c.ColourProperty
            }));

            return Ok(BeadColoursDtos);
        }


        /// <summary>
        /// Return the properties of one specific chosen bead colour in the system
        /// </summary>
        /// <param name="id">The BeadColourId of that chosen colour</param>
        /// <returns>
        /// All the properties releated to that chosen colour, including colour name and the colour property
        /// </returns>
        /// <example>
        /// GET: api/BeadColourData/FindBeadColour/3
        /// </example> 
        [HttpGet]
        [ResponseType(typeof(BeadColour))]
        public IHttpActionResult FindBeadColour(int id)
        {
            BeadColour beadColour = db.BeadsColours.Find(id);
            BeadColourDto BeadColourDto = new BeadColourDto()
            {
                ColourId = beadColour.ColourId,
                ColourName = beadColour.ColourName,
                ColourProperty = beadColour.ColourProperty
            };
            if (beadColour == null)
            {
                return NotFound();
            }

            return Ok(BeadColourDto);
        }

        /// <summary>
        /// Update a specific bead colour in the system with POST Data input
        /// </summary>
        /// <param name="id">The primary key of the specific bead colour</param>
        /// <param name="bead">JSON FORM DATA of a bead colour</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/BeadColourData/UpdateBeadColour/5
        /// </example>
        /// <example>
        /// CREATE: A beadcolourupdate.json file, with JSON FORM DATA that want to input into system
        ///        {
        ///             "ColourId": 3,
        ///             "ColourName": "Black",
        ///             "ColourProperty": "Solid"
        ///         }
        /// COMMAND WINDOW: curl -d @beadcolourupdate.json -H "Content-type:application/json" https://localhost:44386/api/BeadColourData/UpdateBeadColour/3
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateBeadColour(int id, BeadColour beadColour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != beadColour.ColourId)
            {
                return BadRequest();
            }

            db.Entry(beadColour).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeadColourExists(id))
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
        /// Add a bead colour in the system
        /// </summary>
        /// <param name="bead">JSON FORM DATA of a bead colour</param>
        /// <returns>
        /// Header: 201(Created)
        /// Content: BeadId, Bead Data 
        /// or
        /// Header: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/BeadColourData/AddBeadColour
        /// </example>
        /// <example>
        /// CREATE: A beadcolour.json file, with JSON FORM DATA that want to input into system
        ///        {
        ///             "ColourName": "Blue",
        ///             "ColourProperty": "Solid"
        ///         }
        /// COMMAND WINDOW: curl -d @beadcolour.json -H "Content-type:application/json" https://localhost:44386/api/BeadColourData/AddBeadColour       
        /// RETURN:{"ColourId":4,"ColourName":"Blue","ColourProperty":"Solid"}
        /// </example>
        [HttpPost]
        [ResponseType(typeof(BeadColour))]
        public IHttpActionResult AddBeadColour(BeadColour beadColour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BeadsColours.Add(beadColour);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = beadColour.ColourId }, beadColour);
        }



        /// <summary>
        /// Delete one specific bead colour from the system by it's ID
        /// </summary>
        /// <param name="id">The primary key of the bead colour that you want to delete</param>
        /// <returns>
        /// Header: 200 (OK, if it deletes successfully)
        /// or
        /// Header: 404 (NOT FOUND, if it deletes NOT successfully)
        /// </returns>
        /// <example>
        /// Post: api/BeadColourData/DeleteBeadColour/5
        /// curl -d "" "https://localhost:44386/api/BeadColourData/DeleteBeadColour/4"
        /// Form data: (empty)
        /// </example>
        [HttpPost]
        [ResponseType(typeof(BeadColour))]
        public IHttpActionResult DeleteBeadColour(int id)
        {
            BeadColour beadColour = db.BeadsColours.Find(id);
            if (beadColour == null)
            {
                return NotFound();
            }

            db.BeadsColours.Remove(beadColour);
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

        private bool BeadColourExists(int id)
        {
            return db.BeadsColours.Count(e => e.ColourId == id) > 0;
        }
    }
}