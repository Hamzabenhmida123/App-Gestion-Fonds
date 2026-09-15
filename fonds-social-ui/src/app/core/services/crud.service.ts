import { inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/app-config';
import { PagedResult } from '../models/pagination.model';

export abstract class CrudService<T, TCreate = Partial<T>, TUpdate = TCreate> {
  protected http = inject(HttpClient);
  protected abstract endpoint: string;

  private get url(): string {
    return `${API_BASE_URL}/${this.endpoint}`;
  }

  /**
   * Liste paginée côté SQL. `params` peut inclure `page`/`pageSize` (défaut 1/20) et
   * tout filtre supporté par l'endpoint (ex: contratId, seanceComiteId, typeDePretId).
   */
  getAll(params: Record<string, string | number> = {}): Observable<PagedResult<T>> {
    let httpParams = new HttpParams();
    for (const [key, value] of Object.entries(params)) {
      if (value !== undefined && value !== null) httpParams = httpParams.set(key, value);
    }
    if (!httpParams.has('page')) httpParams = httpParams.set('page', 1);
    if (!httpParams.has('pageSize')) httpParams = httpParams.set('pageSize', 20);
    return this.http.get<PagedResult<T>>(this.url, { params: httpParams });
  }

  getById(id: number): Observable<T> {
    return this.http.get<T>(`${this.url}/${id}`);
  }

  create(dto: TCreate): Observable<T> {
    return this.http.post<T>(this.url, dto);
  }

  update(id: number, dto: TUpdate): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
