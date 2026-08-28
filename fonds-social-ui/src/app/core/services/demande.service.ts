import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/app-config';
import { StatutVerification } from '../models/enums';
import { CreateDemande, Demande, DemandeFilters, UploadPieceResult } from '../models/demande.model';

@Injectable({ providedIn: 'root' })
export class DemandeService {
  private http = inject(HttpClient);
  private url = `${API_BASE_URL}/Demande`;

  getAll(filters: DemandeFilters = {}): Observable<Demande[]> {
    let params = new HttpParams();
    if (filters.agentId != null) params = params.set('agentId', filters.agentId);
    if (filters.statut != null) params = params.set('statut', filters.statut);
    if (filters.typeDePretId != null) params = params.set('typeDePretId', filters.typeDePretId);
    if (filters.from) params = params.set('from', filters.from);
    if (filters.to) params = params.set('to', filters.to);
    return this.http.get<Demande[]>(this.url, { params });
  }

  getById(id: number): Observable<Demande> {
    return this.http.get<Demande>(`${this.url}/${id}`);
  }

  create(dto: CreateDemande): Observable<Demande> {
    return this.http.post<Demande>(this.url, dto);
  }

  uploadPiece(demandeId: number, file: File, typePiece: string): Observable<UploadPieceResult> {
    const form = new FormData();
    form.append('file', file);
    form.append('typePiece', typePiece);
    return this.http.post<UploadPieceResult>(`${this.url}/${demandeId}/pieces`, form);
  }

  changePieceStatus(pieceId: number, statutVerification: StatutVerification, auteur: string, commentaire?: string): Observable<void> {
    return this.http.post<void>(`${this.url}/pieces/${pieceId}/status`, { statutVerification, auteur, commentaire });
  }

  cloturerDepot(demandeId: number, auteur: string, commentaire?: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${demandeId}/cloturer-depot`, { auteur, commentaire });
  }

  transition(demandeId: number, newStatut: number, auteur: string, commentaire?: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${demandeId}/transition`, { newStatut, auteur, commentaire });
  }
}
