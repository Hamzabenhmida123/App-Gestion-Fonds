using System;
using System.Collections.Generic;
using FondsSocial.Domain.Common;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Domain.Entities
{
    public class SeanceComite : BaseEntity
    {
        public DateTime Date { get; set; }
        public string? ProcesVerbal { get; set; }
        public StatutVisaSignature StatutVisaSignature { get; set; }

        public virtual ICollection<ParticipationSeance> Participations { get; set; } = new List<ParticipationSeance>();
        public virtual ICollection<Decision> Decisions { get; set; } = new List<Decision>();
    }
}
