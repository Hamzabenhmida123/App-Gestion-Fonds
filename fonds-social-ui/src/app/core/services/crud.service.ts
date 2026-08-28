import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/app-config';

export abstract class CrudService<T, TCreate = Partial<T>, TUpdate = TCreate> {
  protected http = inject(HttpClient);
  protected abstract endpoint: string;

  private get url(): string {
    return `${API_BASE_URL}/${this.endpoint}`;
  }

  getAll(): Observable<T[]> {
    return this.http.get<T[]>(this.url);
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
