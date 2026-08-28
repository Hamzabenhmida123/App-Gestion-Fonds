using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.Repositories;

namespace FondsSocial.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<Societe> Societes { get; }
        IRepository<Agent> Agents { get; }
        IRepository<TypeDePret> TypeDePrets { get; }
        IRepository<PieceJustificativeRequise> PieceJustificativeRequises { get; }
        IRepository<Demande> Demandes { get; }
        IRepository<HistoriqueStatutDemande> HistoriqueStatutDemandes { get; }
        IRepository<PieceJustificative> PieceJustificatives { get; }
        IRepository<MembreComite> MembreComites { get; }
        IRepository<SeanceComite> SeanceComites { get; }
        IRepository<ParticipationSeance> ParticipationSeances { get; }
        IRepository<Decision> Decisions { get; }
        IRepository<Contrat> Contrats { get; }
        IRepository<Garantie> Garanties { get; }
        IRepository<Echeance> Echeances { get; }
        IRepository<RetenueMensuelle> RetenuesMensuelles { get; }
        IRepository<BudgetFonds> BudgetsFonds { get; }

        Task<int> SaveChangesAsync();

        /// <summary>
        /// Ouvre une transaction avec le niveau d'isolation demandé. À utiliser pour les
        /// opérations métier sensibles qui lisent puis écrivent des agrégats partagés
        /// (ex: vérifications d'éligibilité d'une Demande) afin d'éviter les races entre
        /// requêtes concurrentes.
        /// </summary>
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel);
    }
}
