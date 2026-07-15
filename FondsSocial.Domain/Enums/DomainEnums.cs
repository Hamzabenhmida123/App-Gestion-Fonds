namespace FondsSocial.Domain.Enums
{
    public enum SituationFamiliale { Celibataire, Marie, Divorce, Veuf }

    public enum CategorieBudget { Logement, Vehicule, Autre }

    public enum ModeCalculFraisGestion { PourcentageConstant, PourcentageDegressif, MontantFixe }

    public enum StatutDemande
    {
        Deposee,
        Enregistree,
        ConvocationEnvoyee,
        AEtude,
        PVVise,
        Signee,
        DecisionNotifiee,
        RetenueProgrammee,
        DecaisseeCloturee,
        Ajournee,
        Rejetee,
        Caduque
    }

    public enum StatutVerification { EnAttente, Conforme, NonConforme }

    public enum StatutVisaSignature { EnPreparation, EnAttenteVisa, ViseParTousLesMembres, SigneParDG }

    public enum SensDecision { Favorable, Defavorable, Ajourne }

    public enum StatutRetenue { Prevue, Retenue, Anomalie, Annulee }

    public enum StatutEcheance { APayer, Payee, Impayee, Suspendue }
}
