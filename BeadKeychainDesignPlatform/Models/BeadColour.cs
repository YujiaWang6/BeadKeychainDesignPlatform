using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BeadKeychainDesignPlatform.Models
{
    public class BeadColour
    {
        [Key]
        public int ColourId { get; set; }
        public string ColourName { get; set; }
        public string ColourProperty { get; set; }
    }
}