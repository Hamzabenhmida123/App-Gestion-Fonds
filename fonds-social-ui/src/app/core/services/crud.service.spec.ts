import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AgentService } from './agent.service';
import { API_BASE_URL } from '../config/app-config';

// CrudService.getAll() est la logique de pagination partagée par tous les services CRUD
// (Agent, Societe, TypeDePret, Contrat, Decision, Echeance, Garantie, RetenueMensuelle,
// ParticipationSeance, MembreComite, SeanceComite, BudgetFonds, PieceJustificativeRequise).
// On la teste une seule fois via AgentService, un sous-type qui n'ajoute aucune logique propre.
describe('CrudService (via AgentService)', () => {
  let service: AgentService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(AgentService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('getAll defaults to page 1 / pageSize 20 with no params', () => {
    service.getAll().subscribe();

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Agent`);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('page')).toBe('1');
    expect(req.request.params.get('pageSize')).toBe('20');

    req.flush({ items: [], totalCount: 0, page: 1, pageSize: 20 });
  });

  it('getAll forwards custom page/pageSize and any extra filter param', () => {
    service.getAll({ page: 3, pageSize: 50, societeId: 7 }).subscribe();

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Agent`);
    expect(req.request.params.get('page')).toBe('3');
    expect(req.request.params.get('pageSize')).toBe('50');
    expect(req.request.params.get('societeId')).toBe('7');

    req.flush({ items: [], totalCount: 0, page: 3, pageSize: 50 });
  });

  it('getAll resolves with the PagedResult returned by the API', done => {
    service.getAll().subscribe(result => {
      expect(result.items.length).toBe(2);
      expect(result.totalCount).toBe(2);
      done();
    });

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Agent`);
    req.flush({
      items: [{ id: 1 }, { id: 2 }],
      totalCount: 2,
      page: 1,
      pageSize: 20
    });
  });
});
