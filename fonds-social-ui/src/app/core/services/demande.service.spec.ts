import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { DemandeService } from './demande.service';
import { API_BASE_URL } from '../config/app-config';

describe('DemandeService', () => {
  let service: DemandeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(DemandeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('getAll defaults to page 1 / pageSize 20 and omits unset filters', () => {
    service.getAll().subscribe();

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Demande`);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('page')).toBe('1');
    expect(req.request.params.get('pageSize')).toBe('20');
    expect(req.request.params.has('agentId')).toBeFalse();
    expect(req.request.params.has('statut')).toBeFalse();

    req.flush({ items: [], totalCount: 0, page: 1, pageSize: 20 });
  });

  it('getAll forwards provided filters and pagination', () => {
    service.getAll({ agentId: 5, statut: 3, typeDePretId: 2, from: '2026-01-01', to: '2026-12-31', page: 2, pageSize: 10 }).subscribe();

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Demande`);
    expect(req.request.params.get('agentId')).toBe('5');
    expect(req.request.params.get('statut')).toBe('3');
    expect(req.request.params.get('typeDePretId')).toBe('2');
    expect(req.request.params.get('from')).toBe('2026-01-01');
    expect(req.request.params.get('to')).toBe('2026-12-31');
    expect(req.request.params.get('page')).toBe('2');
    expect(req.request.params.get('pageSize')).toBe('10');

    req.flush({ items: [], totalCount: 0, page: 2, pageSize: 10 });
  });

  it('transition posts newStatut/auteur/commentaire to the demande transition endpoint', () => {
    service.transition(7, 3, 'RH', 'ok').subscribe();

    const req = httpMock.expectOne(`${API_BASE_URL}/Demande/7/transition`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ newStatut: 3, auteur: 'RH', commentaire: 'ok' });
    req.flush(null);
  });

  it('cloturerDepot posts to the cloturer-depot endpoint', () => {
    service.cloturerDepot(9, 'RH').subscribe();

    const req = httpMock.expectOne(`${API_BASE_URL}/Demande/9/cloturer-depot`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ auteur: 'RH', commentaire: undefined });
    req.flush(null);
  });
});
