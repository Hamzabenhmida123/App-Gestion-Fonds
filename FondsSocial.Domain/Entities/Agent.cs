using System;
using System.Collections.Generic;
using FondsSocial.Domain.Common;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Domain.Entities
{
    public class Agent : BaseEntity
    {
        public string Matricule { get; set; } = null!;
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public string CIN { get; set; } = null!;
        public string IdentifiantUnique { get; set; } = null!;
        public string? Grade { get; set; }
        public string? Fonction { get; set; }
        public string? Adresse { get; set; }
        public string? Telephone { get; set; }
        public DateTime? DateTitularisation { get; set; }
        public SituationFamiliale? SituationFamiliale { get; set; }
        public int NombreEnfantsACharge { get; set; }

        public int SocieteId { get; set; }
        public virtual Societe Societe { get; set; } = null!;

        public virtual ICollection<Demande> Demandes { get; set; } = new List<Demande>();
        public virtual ICollection<RetenueMensuelle> RetenuesMensuelles { get; set; } = new List<RetenueMensuelle>();
    }
}
