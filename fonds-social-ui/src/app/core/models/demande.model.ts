import { SensDecision, StatutDemande, StatutVerification } from './enums';

export interface HistoriqueStatutDemande {
  id: number;
  demandeId: number;
  statut: StatutDemande;
  dateChangement: string;
  auteur: string;
  commentaire?: string | null;
}

export interface PieceJustificative {
  id: number;
  demandeId: number;
  typePiece: string;
  cheminFichier: string;
  dateDepot: string;
  statutVerification: StatutVerification;
}

export interface Decision {
  id: number;
  demandeId: number;
  seanceComiteId: number;
  sensDecision: SensDecision;
  montantAccorde?: number | null;
  dateNotification?: string | null;
  datePeremption?: string | null;
  contratId?: number | null;
}

export interface Demande {
  id: number;
  numeroDossier: string;
  agentId: number;
  typeDePretId: number;
  dateDepot: string;
  montantDemande: number;
  statutCourant: StatutDemande;
  scorePriorite: number;
  historiqueStatuts?: HistoriqueStatutDemande[];
  pieceJustificatives?: PieceJustificative[];
  decisions?: Decision[];
}

export interface CreateDemande {
  agentId: number;
  typeDePretId: number;
  dateDepot: string;
  montantDemande: number;
}

export interface DemandeFilters {
  agentId?: number;
  statut?: StatutDemande;
  typeDePretId?: number;
  from?: string;
  to?: string;
}

export interface UploadPieceResult {
  missingRequired: string[];
}
