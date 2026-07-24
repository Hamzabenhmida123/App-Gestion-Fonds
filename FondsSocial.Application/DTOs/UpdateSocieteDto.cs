namespace FondsSocial.Application.DTOs
{
    public class UpdateSocieteDto
    {
        public string Code { get; set; } = null!;
        public string RaisonSociale { get; set; } = null!;
        public string? ReferentielReglesGestion { get; set; }
    }
}
