using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.DTOs
{
    public class ChangePieceStatusRequest
    {
        public StatutVerification StatutVerification { get; set; }
        public string Auteur { get; set; } = null!;
        public string? Commentaire { get; set; }
    }
}
