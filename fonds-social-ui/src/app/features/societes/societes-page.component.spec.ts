import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { SocietesPageComponent } from './societes-page.component';
import { API_BASE_URL } from '../../core/config/app-config';

describe('SocietesPageComponent', () => {
  let component: SocietesPageComponent;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SocietesPageComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });

    const fixture = TestBed.createComponent(SocietesPageComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);

    fixture.detectChanges(); // triggers ngOnInit: Societe (list) request

    httpMock.expectOne(r => r.url === `${API_BASE_URL}/Societe`)
      .flush({ items: [], totalCount: 25, page: 1, pageSize: 20 });
  });

  afterEach(() => httpMock.verify());

  it('computes totalPages from totalCount and pageSize', () => {
    expect(component.totalPages()).toBe(2); // ceil(25/20)
  });

  it('nextPage advances the page and requests it with page=2', () => {
    component.nextPage();
    expect(component.page()).toBe(2);

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Societe`);
    expect(req.request.params.get('page')).toBe('2');
    req.flush({ items: [], totalCount: 25, page: 2, pageSize: 20 });
  });

  it('nextPage does nothing once on the last page', () => {
    component.page.set(2);
    component.nextPage();
    expect(component.page()).toBe(2); // unchanged - no HTTP call expected (verified by afterEach)
  });

  it('previousPage does nothing before page 1', () => {
    component.previousPage();
    expect(component.page()).toBe(1); // unchanged - no HTTP call expected
  });
});
