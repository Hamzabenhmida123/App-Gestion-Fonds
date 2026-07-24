using System;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.DTOs
{
    public class UpdatePieceJustificativeDto
    {
        public int DemandeId { get; set; }
        public string TypePiece { get; set; } = null!;
        public string CheminFichier { get; set; } = null!;
        public DateTime DateDepot { get; set; }
        public StatutVerification StatutVerification { get; set; }
    }
}
