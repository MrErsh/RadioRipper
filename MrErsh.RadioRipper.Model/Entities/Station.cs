using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrErsh.RadioRipper.Model
{
    public class Station : Entity
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        public List<Track> Tracks { get; set; }

        public string Comment { get; set; }

        public bool IsRunning { get; set; }
    }
}
