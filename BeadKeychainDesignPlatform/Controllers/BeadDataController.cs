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
using System.Web;
using System.IO;

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
        /// Gather information about all the beads related to a particular colour.
        /// </summary>
        /// <returns>
        /// All the beads in the system with the properties of beads and also the colour and colour property of the beads match with the particular colour ID
        /// </returns>
        /// <param name="id">Colour ID</param>
        /// <example>
        /// GET: api/BeadData/ListBeadsForColour/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(BeadDto))]
        public IHttpActionResult ListBeadsForColour(int id)
        {
            List<Bead> Beads = db.Beads.Where(b=>b.ColourId==id).ToList();
            List<BeadDto> BeadDtos = new List<BeadDto>();

            Beads.ForEach(b => BeadDtos.Add(new BeadDto()
            {
                BeadId = b.BeadId,
                BeadDescription = b.BeadDescription,
                BeadName = b.BeadName,
                BeadPicture = b.BeadPicture,
                ColourName = b.BeadColour.ColourName,
                ColourProperty = b.BeadColour.ColourProperty
            }));

            return Ok(BeadDtos);
        }



        /// <summary>
        /// Gather information about all the beads related to a particular keychain.
        /// </summary>
        /// <returns>
        /// All the beads in the system with the properties of beads and also the colour and colour property of the beads that match to a particular keychain ID
        /// </returns>
        /// <param name="id">Keychain ID</param>
        /// <example>
        /// GET: api/BeadData/ListBeadsForKeychain/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(BeadDto))]
        public IHttpActionResult ListBeadsForKeychain(int id)
        {
            //all beads that have keychians which match with the ID
            List<Bead> Beads = db.Beads.Where(
                b => b.Keychains.Any(
                    k=>k.KeychainId == id
                )).ToList();
            List<BeadDto> BeadDtos = new List<BeadDto>();

            Beads.ForEach(b => BeadDtos.Add(new BeadDto()
            {
                BeadId = b.BeadId,
                BeadDescription = b.BeadDescription,
                BeadName = b.BeadName,
                BeadPicture = b.BeadPicture,
                ColourName = b.BeadColour.ColourName,
                ColourProperty = b.BeadColour.ColourProperty
            }));

            return Ok(BeadDtos);
        }

        /// <summary>
        /// Associate a particular keychain with a particular bead
        /// </summary>
        /// <param name="beadid">The bead id primary key</param>
        /// <param name="keychainid">The keychain id primary key</param>
        /// <returns>
        /// HEADER: 200(OK)
        /// or
        /// HEADER: 404(NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/BeadData/AssociateBeadsWithKeychain/{beadid}/{keychainid}
        /// curl -d "" -v https://localhost:44386/api/BeadData/AssociateBeadsWithKeychain/3/4
        /// </example>
        [HttpPost]
        [Route("api/BeadData/AssociateBeadsWithKeychain/{beadid}/{keychainid}")]
        [Authorize]
        public IHttpActionResult AssociateBeadsWithKeychain(int beadid, int keychainid)
        {
            Bead SelectedBead = db.Beads.Include(b=>b.Keychains).Where(b=>b.BeadId==beadid).FirstOrDefault();
            Keychain SelectedKeychain = db.Keychains.Find(keychainid);

            if(SelectedBead==null || SelectedKeychain == null)
            {
                return NotFound();
            };

            SelectedBead.Keychains.Add(SelectedKeychain);
            db.SaveChanges();

            return Ok();
        }




        /// <summary>
        /// remove an association between a particular keychain and a particular bead
        /// </summary>
        /// <param name="beadid">The bead id primary key</param>
        /// <param name="keychainid">The keychain id primary key</param>
        /// <returns>
        /// HEADER: 200(OK)
        /// or
        /// HEADER: 404(NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/BeadData/RemoveBeadsWithKeychain/{beadid}/{keychainid}
        /// curl -d "" -v https://localhost:44386/api/BeadData/RemoveBeadsWithKeychain/3/4
        /// </example>
        [HttpPost]
        [Route("api/BeadData/RemoveBeadsWithKeychain/{beadid}/{keychainid}")]
        [Authorize]
        public IHttpActionResult RemoveBeadsWithKeychain(int beadid, int keychainid)
        {
            Bead SelectedBead = db.Beads.Include(b => b.Keychains).Where(b => b.BeadId == beadid).FirstOrDefault();
            Keychain SelectedKeychain = db.Keychains.Find(keychainid);

            if (SelectedBead == null || SelectedKeychain == null)
            {
                return NotFound();
            };

            SelectedBead.Keychains.Remove(SelectedKeychain);
            db.SaveChanges();

            return Ok();
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
        [Authorize]
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
            //db.Entry(bead).Property(b => b.BeadPicture).IsModified = false;

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
        /// receives bead picture data and upload it to the webserver and updates the path in database
        /// </summary>
        /// <param name="id">the bead id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F beadPic=@file.jpg "https://localhost:44386/api/BeadData/UploadBeadPic/4"
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// POST: api/BeadData/UploadBeadPic/{id}
        /*[HttpPost]
        public IHttpActionResult UploadBeadPic(int id)
        {
            string picroot;
            if (Request.Content.IsMimeMultipartContent())
            {
                int numfiles = HttpContext.Current.Request.Files.Count;

                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var beadPic = HttpContext.Current.Request.Files[0];
                    if (beadPic.ContentLength > 0)
                    {
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(beadPic.FileName).Substring(1);

                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                string fn = id + "." + extension;

                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/img/beads/"), fn);
                                beadPic.SaveAs(path);
                                picroot = fn;

                                Bead Selectedbead = db.Beads.Find(id);
                                Selectedbead.BeadPicture = picroot;
                                db.Entry(Selectedbead).State = EntityState.Modified;

                                db.SaveChanges();
                            }
                            catch(Exception ex)
                            {
                                return BadRequest();
                            }
                        }
                    }
                }
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        */


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
        [Authorize]
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
        [Authorize]
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