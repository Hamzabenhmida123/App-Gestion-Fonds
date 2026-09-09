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
        private static readonly string[] AllowedPieceExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        private const long MaxPieceSizeBytes = 5 * 1024 * 1024; // 5 Mo

        private readonly IDemandeService _service;
        private readonly IMapper _mapper;

        public DemandeController(IDemandeService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <summary>Liste paginée des demandes avec filtres optionnels</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? agentId, [FromQuery] StatutDemande? statut, [FromQuery] int? typeDePretId,
            [FromQuery] DateTime? from, [FromQuery] DateTime? to,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _service.GetAllFilteredAsync(agentId, statut, typeDePretId, from, to, page, pageSize);
            return Ok(result);
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
            if (file == null || file.Length == 0) return BadRequest(new { error = "Fichier manquant." });
            if (string.IsNullOrWhiteSpace(typePiece)) return BadRequest(new { error = "Type de pièce manquant." });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedPieceExtensions.Contains(extension))
                return BadRequest(new { error = $"Extension non autorisée. Formats acceptés : {string.Join(", ", AllowedPieceExtensions)}." });

            if (file.Length > MaxPieceSizeBytes)
                return BadRequest(new { error = $"Fichier trop volumineux (taille maximale : {MaxPieceSizeBytes / (1024 * 1024)} Mo)." });

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            // Le nom d'origine n'est jamais conservé sur le disque : seul un GUID + l'extension
            // validée composent le nom de fichier stocké, pour éviter toute caractère indésirable.
            var filename = $"{Guid.NewGuid()}{extension}";
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

        /// <summary>
        /// Télécharge le fichier d'une pièce justificative. Remplace l'ancien accès direct
        /// via un middleware de fichiers statiques (qui exposait tout le dossier uploads sans
        /// aucun contrôle) par un accès qui vérifie que la pièce existe réellement en base.
        /// </summary>
        [HttpGet("pieces/{pieceId}/file")]
        public async Task<IActionResult> DownloadPiece(int pieceId)
        {
            var piece = await _service.GetPieceByIdAsync(pieceId);
            if (piece == null) return NotFound();

            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var filename = Path.GetFileName(piece.CheminFichier);
            var fullPath = Path.Combine(uploadsRoot, filename);
            if (!System.IO.File.Exists(fullPath)) return NotFound();

            return PhysicalFile(fullPath, GetContentType(filename), filename);
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
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
}
