import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComitePageComponent } from './comite-page.component';
import { API_BASE_URL } from '../../core/config/app-config';

describe('ComitePageComponent', () => {
  let component: ComitePageComponent;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ComitePageComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });

    const fixture = TestBed.createComponent(ComitePageComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);

    fixture.detectChanges(); // triggers ngOnInit: MembreComite, SeanceComite, Demande

    httpMock.expectOne(r => r.url === `${API_BASE_URL}/MembreComite`)
      .flush({ items: [], totalCount: 8, page: 1, pageSize: 20 });
    httpMock.expectOne(r => r.url === `${API_BASE_URL}/SeanceComite`)
      .flush({ items: [{ id: 1, date: '2026-01-01', statutVisaSignature: 0 }], totalCount: 25, page: 1, pageSize: 20 });

    const demandeReq = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Demande`);
    expect(demandeReq.request.params.get('pageSize')).toBe('100');
    demandeReq.flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });
  });

  afterEach(() => httpMock.verify());

  it('computes membresTotalPages and seancesTotalPages independently', () => {
    expect(component.membresTotalPages()).toBe(1); // ceil(8/20)
    expect(component.seancesTotalPages()).toBe(2); // ceil(25/20)
  });

  it('nextMembresPage advances only the membres page and requests page 2', () => {
    component.membresTotalCount.set(50); // force more than one page
    component.nextMembresPage();
    expect(component.membresPage()).toBe(2);
    expect(component.seancesPage()).toBe(1);

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/MembreComite`);
    expect(req.request.params.get('page')).toBe('2');
    req.flush({ items: [], totalCount: 50, page: 2, pageSize: 20 });
  });

  it('nextSeancesPage advances only the seances page and requests page 2', () => {
    component.nextSeancesPage();
    expect(component.seancesPage()).toBe(2);
    expect(component.membresPage()).toBe(1);

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/SeanceComite`);
    expect(req.request.params.get('page')).toBe('2');
    req.flush({ items: [], totalCount: 25, page: 2, pageSize: 20 });
  });

  it('toggleSeance fetches ParticipationSeance/Decision scoped to the séance id', () => {
    component.toggleSeance(component.seances()[0]);

    const participationReq = httpMock.expectOne(r => r.url === `${API_BASE_URL}/ParticipationSeance`);
    expect(participationReq.request.params.get('seanceComiteId')).toBe('1');
    expect(participationReq.request.params.get('pageSize')).toBe('100');
    participationReq.flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });

    const decisionReq = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Decision`);
    expect(decisionReq.request.params.get('seanceComiteId')).toBe('1');
    expect(decisionReq.request.params.get('pageSize')).toBe('100');
    decisionReq.flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });

    expect(component.expandedSeanceId()).toBe(1);
  });
});
