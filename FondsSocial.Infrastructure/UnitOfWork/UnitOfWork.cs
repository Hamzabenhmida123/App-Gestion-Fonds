using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using FondsSocial.Infrastructure.Repositories;
using FondsSocial.Infrastructure.Data;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Infrastructure.UnitOfWork
{
    public class UnitOfWorkService : IUnitOfWork
    {
        private readonly FondsSocialDbContext _context;
        public UnitOfWorkService(FondsSocialDbContext context)
        {
            _context = context;
            Societes = new Repository<Societe>(context);
            Agents = new Repository<Agent>(context);
            TypeDePrets = new Repository<TypeDePret>(context);
            PieceJustificativeRequises = new Repository<PieceJustificativeRequise>(context);
            Demandes = new Repository<Demande>(context);
            HistoriqueStatutDemandes = new Repository<HistoriqueStatutDemande>(context);
            PieceJustificatives = new Repository<PieceJustificative>(context);
            MembreComites = new Repository<MembreComite>(context);
            SeanceComites = new Repository<SeanceComite>(context);
            ParticipationSeances = new Repository<ParticipationSeance>(context);
            Decisions = new Repository<Decision>(context);
            Contrats = new Repository<Contrat>(context);
            Garanties = new Repository<Garantie>(context);
            Echeances = new Repository<Echeance>(context);
            RetenuesMensuelles = new Repository<RetenueMensuelle>(context);
            BudgetsFonds = new Repository<BudgetFonds>(context);
        }

        public IRepository<Societe> Societes { get; }
        public IRepository<Agent> Agents { get; }
        public IRepository<TypeDePret> TypeDePrets { get; }
        public IRepository<PieceJustificativeRequise> PieceJustificativeRequises { get; }
        public IRepository<Demande> Demandes { get; }
        public IRepository<HistoriqueStatutDemande> HistoriqueStatutDemandes { get; }
        public IRepository<PieceJustificative> PieceJustificatives { get; }
        public IRepository<MembreComite> MembreComites { get; }
        public IRepository<SeanceComite> SeanceComites { get; }
        public IRepository<ParticipationSeance> ParticipationSeances { get; }
        public IRepository<Decision> Decisions { get; }
        public IRepository<Contrat> Contrats { get; }
        public IRepository<Garantie> Garanties { get; }
        public IRepository<Echeance> Echeances { get; }
        public IRepository<RetenueMensuelle> RetenuesMensuelles { get; }
        public IRepository<BudgetFonds> BudgetsFonds { get; }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            return _context.Database.BeginTransactionAsync(isolationLevel);
        }
    }
}
