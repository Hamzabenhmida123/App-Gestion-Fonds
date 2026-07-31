using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using FondsSocial.Application.DTOs;
using FondsSocial.Application.Services;
using FondsSocial.Domain.Enums;

namespace FondsSocial.API.Controllers
{
    /// <summary>
    /// Endpoints pour gérer les demandes (Module M1)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DemandeController : ControllerBase
    {
        private readonly IDemandeService _service;
        private readonly IMapper _mapper;

        public DemandeController(IDemandeService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <summary>Liste des demandes avec filtres optionnels</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? agentId, [FromQuery] StatutDemande? statut, [FromQuery] int? typeDePretId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var list = await _service.GetAllFilteredAsync(agentId, statut, typeDePretId, from, to);
            return Ok(list);
        }

        /// <summary>Récupère une demande complète (avec historique et pièces)</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _service.GetByIdWithDetailsAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        /// <summary>Créer une nouvelle demande (vérifications d'éligibilité effectuées)</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDemandeDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Déposer une pièce justificative (multipart/form-data)</summary>
        [HttpPost("{id}/pieces")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPiece(int id, [FromForm] IFormFile file, [FromForm] string typePiece)
        {
            if (file == null || file.Length == 0) return BadRequest("Fichier manquant");

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var filename = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var full = Path.Combine(uploads, filename);
            using (var stream = System.IO.File.Create(full))
            {
                await file.CopyToAsync(stream);
            }

            var createDto = new CreatePieceJustificativeDto
            {
                DemandeId = id,
                TypePiece = typePiece,
                CheminFichier = $"/uploads/{filename}",
                DateDepot = DateTime.UtcNow,
                StatutVerification = StatutVerification.EnAttente
            };

            var missing = await _service.AddPieceRecordAsync(createDto);
            return Ok(new { missingRequired = missing });
        }

        /// <summary>Changer le statut d'une pièce</summary>
        [HttpPost("pieces/{pieceId}/status")]
        public async Task<IActionResult> ChangePieceStatus(int pieceId, [FromBody] ChangePieceStatusRequest req)
        {
            var ok = await _service.ChangePieceStatusAsync(pieceId, req.StatutVerification, req.Auteur, req.Commentaire ?? string.Empty);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>Effectuer une transition de statut pour une demande</summary>
        [HttpPost("{id}/transition")]
        public async Task<IActionResult> Transition(int id, [FromBody] TransitionRequest req)
        {
            var ok = await _service.TransitionStatutAsync(id, req.NewStatut, req.Auteur, req.Commentaire ?? string.Empty);
            if (!ok) return BadRequest(new { error = "Transition interdite ou demande introuvable" });
            return NoContent();
        }

        /// <summary>Clôturer le dépôt : vérifie la complétude et la conformité des pièces, puis enregistre la demande</summary>
        [HttpPost("{id}/cloturer-depot")]
        public async Task<IActionResult> CloturerDepot(int id, [FromBody] CloturerDepotRequest req)
        {
            try
            {
                var ok = await _service.CloturerDepotAsync(id, req.Auteur, req.Commentaire ?? string.Empty);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class CloturerDepotRequest
    {
        public string Auteur { get; set; } = null!;
        public string? Commentaire { get; set; }
    }

    public class ChangePieceStatusRequest
    {
        public StatutVerification StatutVerification { get; set; }
        public string Auteur { get; set; } = null!;
        public string? Commentaire { get; set; }
    }

    public class TransitionRequest
    {
        public StatutDemande NewStatut { get; set; }
        public string Auteur { get; set; } = null!;
        public string? Commentaire { get; set; }
    }
}
