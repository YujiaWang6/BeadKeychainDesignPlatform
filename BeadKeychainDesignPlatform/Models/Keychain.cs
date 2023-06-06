using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BeadKeychainDesignPlatform.Models
{
    public class Keychain
    {

        [Key]
        public int KeychainId { get; set; }
        public string KeychainName { get; set; }

        //A keychain can have many beads
        public ICollection<Bead> Beads { get; set; }
    }
}