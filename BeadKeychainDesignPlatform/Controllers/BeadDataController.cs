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
    public class BeadDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/BeadData/ListBeads
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

        // GET: api/BeadData/5
        [ResponseType(typeof(Bead))]
        public IHttpActionResult GetBead(int id)
        {
            Bead bead = db.Beads.Find(id);
            if (bead == null)
            {
                return NotFound();
            }

            return Ok(bead);
        }

        // PUT: api/BeadData/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBead(int id, Bead bead)
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

        // POST: api/BeadData
        [ResponseType(typeof(Bead))]
        public IHttpActionResult PostBead(Bead bead)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Beads.Add(bead);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = bead.BeadId }, bead);
        }

        // DELETE: api/BeadData/5
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

            return Ok(bead);
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