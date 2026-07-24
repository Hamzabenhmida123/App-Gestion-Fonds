using System.Threading.Tasks;
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
    }
}
