export enum SituationFamiliale {
  Celibataire = 0,
  Marie = 1,
  Divorce = 2,
  Veuf = 3
}

export const SITUATION_FAMILIALE_LABELS: Record<SituationFamiliale, string> = {
  [SituationFamiliale.Celibataire]: 'Célibataire',
  [SituationFamiliale.Marie]: 'Marié(e)',
  [SituationFamiliale.Divorce]: 'Divorcé(e)',
  [SituationFamiliale.Veuf]: 'Veuf/Veuve'
};

export enum CategorieBudget {
  Logement = 0,
  Vehicule = 1,
  Autre = 2
}

export const CATEGORIE_BUDGET_LABELS: Record<CategorieBudget, string> = {
  [CategorieBudget.Logement]: 'Logement',
  [CategorieBudget.Vehicule]: 'Véhicule',
  [CategorieBudget.Autre]: 'Autre'
};

export enum ModeCalculFraisGestion {
  PourcentageConstant = 0,
  PourcentageDegressif = 1,
  MontantFixe = 2
}

export const MODE_CALCUL_FRAIS_LABELS: Record<ModeCalculFraisGestion, string> = {
  [ModeCalculFraisGestion.PourcentageConstant]: 'Pourcentage constant',
  [ModeCalculFraisGestion.PourcentageDegressif]: 'Pourcentage dégressif',
  [ModeCalculFraisGestion.MontantFixe]: 'Montant fixe'
};

export enum StatutDemande {
  Deposee = 0,
  Enregistree = 1,
  ConvocationEnvoyee = 2,
  AEtude = 3,
  PVVise = 4,
  Signee = 5,
  DecisionNotifiee = 6,
  RetenueProgrammee = 7,
  DecaisseeCloturee = 8,
  Ajournee = 9,
  Rejetee = 10,
  Caduque = 11
}

export const STATUT_DEMANDE_LABELS: Record<StatutDemande, string> = {
  [StatutDemande.Deposee]: 'Déposée',
  [StatutDemande.Enregistree]: 'Enregistrée',
  [StatutDemande.ConvocationEnvoyee]: 'Convocation envoyée',
  [StatutDemande.AEtude]: 'À l\'étude',
  [StatutDemande.PVVise]: 'PV visé',
  [StatutDemande.Signee]: 'Signée',
  [StatutDemande.DecisionNotifiee]: 'Décision notifiée',
  [StatutDemande.RetenueProgrammee]: 'Retenue programmée',
  [StatutDemande.DecaisseeCloturee]: 'Décaissée / Clôturée',
  [StatutDemande.Ajournee]: 'Ajournée',
  [StatutDemande.Rejetee]: 'Rejetée',
  [StatutDemande.Caduque]: 'Caduque'
};

export const STATUT_DEMANDE_COLORS: Record<StatutDemande, string> = {
  [StatutDemande.Deposee]: 'var(--color-bg-secondary)',
  [StatutDemande.Enregistree]: 'var(--color-secondary-blue)',
  [StatutDemande.ConvocationEnvoyee]: 'var(--color-secondary-blue)',
  [StatutDemande.AEtude]: 'var(--color-primary)',
  [StatutDemande.PVVise]: 'var(--color-secondary-purple)',
  [StatutDemande.Signee]: 'var(--color-secondary-purple)',
  [StatutDemande.DecisionNotifiee]: 'var(--color-secondary-teal)',
  [StatutDemande.RetenueProgrammee]: 'var(--color-secondary-teal)',
  [StatutDemande.DecaisseeCloturee]: 'var(--color-success)',
  [StatutDemande.Ajournee]: 'var(--color-bg-secondary)',
  [StatutDemande.Rejetee]: 'var(--color-danger)',
  [StatutDemande.Caduque]: 'var(--color-bg-secondary)'
};

export const STATUT_DEMANDE_TRANSITIONS: Record<StatutDemande, StatutDemande[]> = {
  [StatutDemande.Deposee]: [StatutDemande.Enregistree, StatutDemande.AEtude, StatutDemande.Rejetee],
  [StatutDemande.Enregistree]: [StatutDemande.AEtude, StatutDemande.Rejetee],
  [StatutDemande.ConvocationEnvoyee]: [],
  [StatutDemande.AEtude]: [StatutDemande.PVVise, StatutDemande.Ajournee, StatutDemande.Rejetee],
  [StatutDemande.PVVise]: [StatutDemande.Signee, StatutDemande.Rejetee],
  [StatutDemande.Signee]: [StatutDemande.DecisionNotifiee],
  [StatutDemande.DecisionNotifiee]: [StatutDemande.RetenueProgrammee, StatutDemande.DecaisseeCloturee],
  [StatutDemande.RetenueProgrammee]: [],
  [StatutDemande.DecaisseeCloturee]: [],
  [StatutDemande.Ajournee]: [],
  [StatutDemande.Rejetee]: [],
  [StatutDemande.Caduque]: []
};

export enum StatutVerification {
  EnAttente = 0,
  Conforme = 1,
  NonConforme = 2
}

export const STATUT_VERIFICATION_LABELS: Record<StatutVerification, string> = {
  [StatutVerification.EnAttente]: 'En attente',
  [StatutVerification.Conforme]: 'Conforme',
  [StatutVerification.NonConforme]: 'Non conforme'
};

export const STATUT_VERIFICATION_COLORS: Record<StatutVerification, string> = {
  [StatutVerification.EnAttente]: 'var(--color-bg-secondary)',
  [StatutVerification.Conforme]: 'var(--color-success)',
  [StatutVerification.NonConforme]: 'var(--color-danger)'
};

export enum StatutVisaSignature {
  EnPreparation = 0,
  EnAttenteVisa = 1,
  ViseParTousLesMembres = 2,
  SigneParDG = 3
}

export const STATUT_VISA_LABELS: Record<StatutVisaSignature, string> = {
  [StatutVisaSignature.EnPreparation]: 'En préparation',
  [StatutVisaSignature.EnAttenteVisa]: 'En attente de visa',
  [StatutVisaSignature.ViseParTousLesMembres]: 'Visé par tous les membres',
  [StatutVisaSignature.SigneParDG]: 'Signé par le DG'
};

export enum SensDecision {
  Favorable = 0,
  Defavorable = 1,
  Ajourne = 2
}

export const SENS_DECISION_LABELS: Record<SensDecision, string> = {
  [SensDecision.Favorable]: 'Favorable',
  [SensDecision.Defavorable]: 'Défavorable',
  [SensDecision.Ajourne]: 'Ajournée'
};

export const SENS_DECISION_COLORS: Record<SensDecision, string> = {
  [SensDecision.Favorable]: 'var(--color-success)',
  [SensDecision.Defavorable]: 'var(--color-danger)',
  [SensDecision.Ajourne]: 'var(--color-bg-secondary)'
};

export enum StatutRetenue {
  Prevue = 0,
  Retenue = 1,
  Anomalie = 2,
  Annulee = 3
}

export const STATUT_RETENUE_LABELS: Record<StatutRetenue, string> = {
  [StatutRetenue.Prevue]: 'Prévue',
  [StatutRetenue.Retenue]: 'Retenue',
  [StatutRetenue.Anomalie]: 'Anomalie',
  [StatutRetenue.Annulee]: 'Annulée'
};

export enum StatutEcheance {
  APayer = 0,
  Payee = 1,
  Impayee = 2,
  Suspendue = 3
}

export const STATUT_ECHEANCE_LABELS: Record<StatutEcheance, string> = {
  [StatutEcheance.APayer]: 'À payer',
  [StatutEcheance.Payee]: 'Payée',
  [StatutEcheance.Impayee]: 'Impayée',
  [StatutEcheance.Suspendue]: 'Suspendue'
};

export function enumOptions<T extends number>(labels: Record<T, string>): { value: T; label: string }[] {
  return Object.entries(labels).map(([value, label]) => ({ value: Number(value) as T, label: label as string }));
}
