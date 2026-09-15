import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { DemandeListComponent } from './demande-list.component';
import { API_BASE_URL } from '../../core/config/app-config';

describe('DemandeListComponent', () => {
  let component: DemandeListComponent;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [DemandeListComponent],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])]
    });

    const fixture = TestBed.createComponent(DemandeListComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);

    fixture.detectChanges(); // triggers ngOnInit: Agent, TypeDePret and Demande requests

    httpMock.expectOne(r => r.url === `${API_BASE_URL}/Agent`).flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });
    httpMock.expectOne(r => r.url === `${API_BASE_URL}/TypeDePret`).flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });
    httpMock.expectOne(r => r.url === `${API_BASE_URL}/Demande`)
      .flush({ items: [], totalCount: 45, page: 1, pageSize: 20 });
  });

  afterEach(() => httpMock.verify());

  it('computes totalPages from totalCount and pageSize', () => {
    expect(component.totalPages()).toBe(3); // ceil(45/20)
  });

  it('nextPage advances the page and reloads with the new page number', () => {
    component.nextPage();
    expect(component.page()).toBe(2);

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Demande`);
    expect(req.request.params.get('page')).toBe('2');
    req.flush({ items: [], totalCount: 45, page: 2, pageSize: 20 });
  });

  it('nextPage does nothing once on the last page', () => {
    component.page.set(3);
    component.nextPage();
    expect(component.page()).toBe(3); // unchanged - no HTTP call expected (verified by afterEach)
  });

  it('previousPage does nothing before page 1', () => {
    component.previousPage();
    expect(component.page()).toBe(1); // unchanged - no HTTP call expected
  });

  it('applyFilters resets to page 1 and includes the active filter', () => {
    component.page.set(2);
    component.filterAgentId = 7;
    component.applyFilters();

    expect(component.page()).toBe(1);
    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Demande`);
    expect(req.request.params.get('agentId')).toBe('7');
    expect(req.request.params.get('page')).toBe('1');
    req.flush({ items: [], totalCount: 45, page: 1, pageSize: 20 });
  });

  it('resetFilters clears every filter and reloads from page 1', () => {
    component.filterAgentId = 3;
    component.filterStatut = 2;
    component.page.set(2);

    component.resetFilters();

    expect(component.filterAgentId).toBeNull();
    expect(component.filterStatut).toBeNull();
    expect(component.page()).toBe(1);

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Demande`);
    expect(req.request.params.has('agentId')).toBeFalse();
    req.flush({ items: [], totalCount: 0, page: 1, pageSize: 20 });
  });
});
