import { Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'fondsSocial.currentAuteur';

/**
 * En l'absence du module de gestion des accès (intégré ultérieurement), l'identité de
 * l'auteur des actions (transition, clôture de dépôt, vérification de pièce) reste
 * déclarative plutôt qu'authentifiée. Ce service centralise sa saisie en un seul endroit
 * persistant (localStorage) au lieu de champs libres dupliqués partout, ou d'un prompt()
 * natif du navigateur pour la vérification des pièces.
 */
@Injectable({ providedIn: 'root' })
export class CurrentUserService {
  private readonly _auteur = signal<string>(readStored());
  readonly auteur = this._auteur.asReadonly();

  setAuteur(value: string): void {
    const trimmed = value.trim();
    this._auteur.set(trimmed);
    try {
      localStorage.setItem(STORAGE_KEY, trimmed);
    } catch {
      // localStorage indisponible (navigation privée, quota, etc.): on garde la valeur en mémoire.
    }
  }
}

function readStored(): string {
  try {
    return localStorage.getItem(STORAGE_KEY) ?? '';
  } catch {
    return '';
  }
}
