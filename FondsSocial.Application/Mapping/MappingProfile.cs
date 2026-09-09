using AutoMapper;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;

namespace FondsSocial.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TypeDePret, TypeDePretDto>().ReverseMap();
            CreateMap<CreateTypeDePretDto, TypeDePret>();
            CreateMap<UpdateTypeDePretDto, TypeDePret>();

            CreateMap<Agent, AgentDto>().ReverseMap();
            CreateMap<CreateAgentDto, Agent>();
            CreateMap<UpdateAgentDto, Agent>();

            CreateMap<Societe, SocieteDto>().ReverseMap();
            CreateMap<CreateSocieteDto, Societe>();
            CreateMap<UpdateSocieteDto, Societe>();

            CreateMap<PieceJustificativeRequise, PieceJustificativeRequiseDto>().ReverseMap();
            CreateMap<CreatePieceJustificativeRequiseDto, PieceJustificativeRequise>();
            CreateMap<UpdatePieceJustificativeRequiseDto, PieceJustificativeRequise>();

            CreateMap<Demande, DemandeDto>().ReverseMap();
            CreateMap<CreateDemandeDto, Demande>();
            CreateMap<UpdateDemandeDto, Demande>();

            CreateMap<HistoriqueStatutDemande, HistoriqueStatutDemandeDto>().ReverseMap();
            CreateMap<PieceJustificative, PieceJustificativeDto>().ReverseMap();
            CreateMap<Decision, DecisionDto>().ReverseMap();

            CreateMap<PieceJustificative, PieceJustificativeDto>().ReverseMap();
            CreateMap<CreatePieceJustificativeDto, PieceJustificative>();
            CreateMap<UpdatePieceJustificativeDto, PieceJustificative>();

            CreateMap<MembreComite, MembreComiteDto>().ReverseMap();
            CreateMap<CreateMembreComiteDto, MembreComite>();
            CreateMap<UpdateMembreComiteDto, MembreComite>();

            CreateMap<SeanceComite, SeanceComiteDto>().ReverseMap();
            CreateMap<CreateSeanceComiteDto, SeanceComite>();
            CreateMap<UpdateSeanceComiteDto, SeanceComite>();

            CreateMap<ParticipationSeance, ParticipationSeanceDto>().ReverseMap();
            CreateMap<CreateParticipationSeanceDto, ParticipationSeance>();
            CreateMap<UpdateParticipationSeanceDto, ParticipationSeance>();

            CreateMap<Decision, DecisionDto>().ReverseMap();
            CreateMap<CreateDecisionDto, Decision>();
            CreateMap<UpdateDecisionDto, Decision>();

            CreateMap<Contrat, ContratDto>().ReverseMap();
            CreateMap<CreateContratDto, Contrat>();
            CreateMap<UpdateContratDto, Contrat>();

            CreateMap<Garantie, GarantieDto>().ReverseMap();
            CreateMap<CreateGarantieDto, Garantie>();
            CreateMap<UpdateGarantieDto, Garantie>();

            CreateMap<Echeance, EcheanceDto>().ReverseMap();
            CreateMap<CreateEcheanceDto, Echeance>();
            CreateMap<UpdateEcheanceDto, Echeance>();

            CreateMap<RetenueMensuelle, RetenueMensuelleDto>().ReverseMap();
            CreateMap<CreateRetenueMensuelleDto, RetenueMensuelle>();
            CreateMap<UpdateRetenueMensuelleDto, RetenueMensuelle>();

            CreateMap<BudgetFonds, BudgetFondsDto>().ReverseMap();
            CreateMap<CreateBudgetFondsDto, BudgetFonds>();
            CreateMap<UpdateBudgetFondsDto, BudgetFonds>();
        }
    }
}
