using System;
using FondsSocial.Domain.Common;

namespace FondsSocial.Domain.Entities
{
    public class ParticipationSeance : BaseEntity
    {
        public int SeanceComiteId { get; set; }
        public int MembreComiteId { get; set; }
        public bool AVise { get; set; }
        public DateTime? DateVisa { get; set; }

        public virtual SeanceComite SeanceComite { get; set; } = null!;
        public virtual MembreComite MembreComite { get; set; } = null!;
    }
}
