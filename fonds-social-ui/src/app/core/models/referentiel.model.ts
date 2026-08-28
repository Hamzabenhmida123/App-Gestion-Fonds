import { SituationFamiliale } from './enums';

export interface Societe {
  id: number;
  code: string;
  raisonSociale: string;
  referentielReglesGestion?: string | null;
}

export type CreateSociete = Omit<Societe, 'id'>;

export interface Agent {
  id: number;
  matricule: string;
  nom: string;
  prenom: string;
  cin: string;
  identifiantUnique: string;
  grade?: string | null;
  fonction?: string | null;
  adresse?: string | null;
  telephone?: string | null;
  dateTitularisation?: string | null;
  situationFamiliale?: SituationFamiliale | null;
  nombreEnfantsACharge: number;
  salaireMensuel?: number | null;
  societeId: number;
}

export type CreateAgent = Omit<Agent, 'id'>;

export interface TypeDePret {
  id: number;
  code: string;
  libelle: string;
  categorie: number;
  plafond: number;
  dureeMaxMois: number;
  franchiseMois: number;
  modeCalculFraisGestion: number;
  tauxOuMontantFraisGestion: number;
  actif: boolean;
}

export type CreateTypeDePret = Omit<TypeDePret, 'id'>;

export interface PieceJustificativeRequise {
  id: number;
  typeDePretId: number;
  libellePiece: string;
  obligatoire: boolean;
}

export type CreatePieceJustificativeRequise = Omit<PieceJustificativeRequise, 'id'>;
