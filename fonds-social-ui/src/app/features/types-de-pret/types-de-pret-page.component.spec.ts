import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TypesDePretPageComponent } from './types-de-pret-page.component';
import { API_BASE_URL } from '../../core/config/app-config';

describe('TypesDePretPageComponent', () => {
  let component: TypesDePretPageComponent;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [TypesDePretPageComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });

    const fixture = TestBed.createComponent(TypesDePretPageComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);

    fixture.detectChanges(); // triggers ngOnInit: TypeDePret (list) request

    httpMock.expectOne(r => r.url === `${API_BASE_URL}/TypeDePret`)
      .flush({
        items: [{ id: 1, code: 'LOG', libelle: 'Logement' }],
        totalCount: 12,
        page: 1,
        pageSize: 20
      });
  });

  afterEach(() => httpMock.verify());

  it('computes totalPages from totalCount and pageSize', () => {
    expect(component.totalPages()).toBe(1); // ceil(12/20)
  });

  it('nextPage does nothing once on the last page', () => {
    component.nextPage();
    expect(component.page()).toBe(1); // already on (and past) the only page - no HTTP call expected
  });

  it('toggleRequiredDocs fetches PieceJustificativeRequise filtered by typeDePretId', () => {
    component.toggleRequiredDocs(component.types()[0]);

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/PieceJustificativeRequise`);
    expect(req.request.params.get('typeDePretId')).toBe('1');
    expect(req.request.params.get('pageSize')).toBe('100');
    req.flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });

    expect(component.expandedTypeId()).toBe(1);
  });

  it('toggleRequiredDocs collapses the row when clicked again', () => {
    component.toggleRequiredDocs(component.types()[0]);
    httpMock.expectOne(r => r.url === `${API_BASE_URL}/PieceJustificativeRequise`)
      .flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });

    component.toggleRequiredDocs(component.types()[0]);
    expect(component.expandedTypeId()).toBeNull(); // no new HTTP call expected
  });
});
