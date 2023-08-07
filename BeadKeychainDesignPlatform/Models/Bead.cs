using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeadKeychainDesignPlatform.Models
{
    public class Bead
    {
        [Key]
        public int BeadId { get; set; }
        public string BeadName { get; set;}
        public string BeadDescription { get; set;}
        public string BeadPicture { get; set; }


        //A bead has one base colour
        [ForeignKey("BeadColour")]
        public int ColourId { get; set; }
        public virtual BeadColour BeadColour { get; set; }

        //A bead can be in many keychains
        public ICollection<Keychain> Keychains { get; set; }


    }

    public class BeadDto
    {
        public int BeadId { get; set; }
        public string BeadName { get; set; }
        public string BeadDescription { get; set; }
        public string BeadPicture { get; set; }
        public int ColourId { get; set; }
        public string ColourName { get; set; }
        public string ColourProperty { get; set; }
    }
}