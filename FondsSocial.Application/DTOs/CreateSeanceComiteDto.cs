using System;
using FondsSocial.Domain.Enums;

namespace FondsSocial.Application.DTOs
{
    public class CreateSeanceComiteDto
    {
        public DateTime Date { get; set; }
        public string? ProcesVerbal { get; set; }
        public StatutVisaSignature StatutVisaSignature { get; set; }
    }
}
