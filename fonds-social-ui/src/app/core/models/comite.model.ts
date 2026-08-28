import { StatutVisaSignature } from './enums';

export interface MembreComite {
  id: number;
  nom: string;
  prenom: string;
  fonction?: string | null;
  actif: boolean;
}

export type CreateMembreComite = Omit<MembreComite, 'id'>;

export interface SeanceComite {
  id: number;
  date: string;
  procesVerbal?: string | null;
  statutVisaSignature: StatutVisaSignature;
}

export type CreateSeanceComite = Omit<SeanceComite, 'id'>;

export interface ParticipationSeance {
  id: number;
  seanceComiteId: number;
  membreComiteId: number;
  aVise: boolean;
  dateVisa?: string | null;
}

export type CreateParticipationSeance = Omit<ParticipationSeance, 'id'>;
