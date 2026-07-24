namespace FondsSocial.Application.DTOs
{
    public class MembreComiteDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public string? Fonction { get; set; }
        public bool Actif { get; set; }
    }
}
