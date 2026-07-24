namespace FondsSocial.Application.DTOs
{
    public class AgentDto
    {
        public int Id { get; set; }
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
        public int NombreEnfantsACharge { get; set; }
        public int SocieteId { get; set; }
    }
}
