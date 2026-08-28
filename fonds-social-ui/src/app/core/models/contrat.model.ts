import { StatutEcheance, StatutRetenue } from './enums';

export interface Contrat {
  id: number;
  decisionId: number;
  dateSignature: string;
  montantPrincipal: number;
  fraisGestion: number;
  montantTotal: number;
  dureeMois: number;
  datePremiereEcheance: string;
}

export type CreateContrat = Omit<Contrat, 'id'>;

export interface Garantie {
  id: number;
  contratId: number;
  assuranceVieSouscrite: boolean;
  dateAssuranceVie?: string | null;
  referencePoliceAssurance?: string | null;
  traiteSigneeLegalisee: boolean;
  dateTraite?: string | null;
  engagementRemboursementSigne: boolean;
  dateEngagement?: string | null;
  autorisationRetenueSalaireSignee: boolean;
  dateAutorisationRetenue?: string | null;
}

export type CreateGarantie = Omit<Garantie, 'id'>;

export interface Echeance {
  id: number;
  contratId: number;
  numeroEcheance: number;
  date: string;
  capitalRestantDu: number;
  mensualite: number;
  capitalAmorti: number;
  fraisGestion: number;
  soldeRestant: number;
  statut: StatutEcheance;
}

export type CreateEcheance = Omit<Echeance, 'id'>;

export interface RetenueMensuelle {
  id: number;
  mois: string;
  agentId: number;
  contratId: number;
  montantARetenir: number;
  montantEffectivementRetenu?: number | null;
  statut: StatutRetenue;
  referencePaie?: string | null;
}

export type CreateRetenueMensuelle = Omit<RetenueMensuelle, 'id'>;
