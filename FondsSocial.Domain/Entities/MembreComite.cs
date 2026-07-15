using System.Collections.Generic;
using FondsSocial.Domain.Common;

namespace FondsSocial.Domain.Entities
{
    public class MembreComite : BaseEntity
    {
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public string? Fonction { get; set; }
        public bool Actif { get; set; }

        public virtual ICollection<ParticipationSeance> Participations { get; set; } = new List<ParticipationSeance>();
    }
}
