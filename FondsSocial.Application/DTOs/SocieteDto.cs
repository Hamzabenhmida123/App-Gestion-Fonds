namespace FondsSocial.Application.DTOs
{
    public class SocieteDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string RaisonSociale { get; set; } = null!;
        public string? ReferentielReglesGestion { get; set; }
    }
}
