import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AgentsPageComponent } from './agents-page.component';
import { API_BASE_URL } from '../../core/config/app-config';

describe('AgentsPageComponent', () => {
  let component: AgentsPageComponent;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [AgentsPageComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });

    const fixture = TestBed.createComponent(AgentsPageComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);

    fixture.detectChanges(); // triggers ngOnInit: Societe (dropdown) and Agent (list) requests

    const societeReq = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Societe`);
    expect(societeReq.request.params.get('pageSize')).toBe('100');
    societeReq.flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });

    httpMock.expectOne(r => r.url === `${API_BASE_URL}/Agent`)
      .flush({ items: [], totalCount: 42, page: 1, pageSize: 20 });
  });

  afterEach(() => httpMock.verify());

  it('computes totalPages from totalCount and pageSize', () => {
    expect(component.totalPages()).toBe(3); // ceil(42/20)
  });

  it('nextPage advances the page and reloads with the new page number', () => {
    component.nextPage();
    expect(component.page()).toBe(2);

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Agent`);
    expect(req.request.params.get('page')).toBe('2');
    req.flush({ items: [], totalCount: 42, page: 2, pageSize: 20 });
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
});
