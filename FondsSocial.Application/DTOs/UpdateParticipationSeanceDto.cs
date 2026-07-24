using System;

namespace FondsSocial.Application.DTOs
{
    public class UpdateParticipationSeanceDto
    {
        public int SeanceComiteId { get; set; }
        public int MembreComiteId { get; set; }
        public bool AVise { get; set; }
        public DateTime? DateVisa { get; set; }
    }
}
