using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeadKeychainDesignPlatform.Models.ViewModels
{
    public class DetailsBeadColour
    {
        //specific bead colour
        public BeadColourDto specificColour { get; set; }


        //all the beads related to this specific colour
        public IEnumerable<BeadDto> relatedBeads { get; set; }
    }
}