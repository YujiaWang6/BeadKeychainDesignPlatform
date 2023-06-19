using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeadKeychainDesignPlatform.Models.ViewModels
{
    public class UpdateBead
    {
        //This viewmodel is a class which stores information that we need to present to /Bead/Edit


        //existing bead information
        public BeadDto SelectedBead { get; set; }

        //include all the bead colour to choose from when update the bead
        public IEnumerable<BeadColourDto> beadcoloursOptions { get; set; }



    }
}