using System;
using System.ComponentModel.DataAnnotations;

namespace MrErsh.RadioRipper.Model
{
    public abstract class Entity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
