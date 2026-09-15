import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { FinancePageComponent } from './finance-page.component';
import { API_BASE_URL } from '../../core/config/app-config';

describe('FinancePageComponent', () => {
  let component: FinancePageComponent;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [FinancePageComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });

    const fixture = TestBed.createComponent(FinancePageComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);

    fixture.detectChanges(); // triggers ngOnInit: Contrat, BudgetFonds, Decision, Agent, Societe

    httpMock.expectOne(r => r.url === `${API_BASE_URL}/Contrat`)
      .flush({ items: [{ id: 1, decisionId: 1, dateSignature: '2026-01-01', montantPrincipal: 1000, montantTotal: 1100, dureeMois: 12 }], totalCount: 30, page: 1, pageSize: 20 });
    httpMock.expectOne(r => r.url === `${API_BASE_URL}/BudgetFonds`)
      .flush({ items: [], totalCount: 15, page: 1, pageSize: 20 });

    const decisionReq = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Decision`);
    expect(decisionReq.request.params.get('pageSize')).toBe('100');
    decisionReq.flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });

    httpMock.expectOne(r => r.url === `${API_BASE_URL}/Agent`).flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });
    httpMock.expectOne(r => r.url === `${API_BASE_URL}/Societe`).flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });
  });

  afterEach(() => httpMock.verify());

  it('computes contratsTotalPages and budgetsTotalPages independently', () => {
    expect(component.contratsTotalPages()).toBe(2); // ceil(30/20)
    expect(component.budgetsTotalPages()).toBe(1); // ceil(15/20)
  });

  it('nextContratsPage advances only the contrats page and requests page 2', () => {
    component.nextContratsPage();
    expect(component.contratsPage()).toBe(2);
    expect(component.budgetsPage()).toBe(1);

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Contrat`);
    expect(req.request.params.get('page')).toBe('2');
    req.flush({ items: [], totalCount: 30, page: 2, pageSize: 20 });
  });

  it('nextBudgetsPage advances only the budgets page and requests page 2', () => {
    component.budgetsTotalCount.set(50); // force more than one page
    component.nextBudgetsPage();
    expect(component.budgetsPage()).toBe(2);
    expect(component.contratsPage()).toBe(1);

    const req = httpMock.expectOne(r => r.url === `${API_BASE_URL}/BudgetFonds`);
    expect(req.request.params.get('page')).toBe('2');
    req.flush({ items: [], totalCount: 50, page: 2, pageSize: 20 });
  });

  it('toggleContrat fetches Garantie/Echeance/RetenueMensuelle scoped to the contrat id', () => {
    component.toggleContrat(component.contrats()[0]);

    const garantieReq = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Garantie`);
    expect(garantieReq.request.params.get('contratId')).toBe('1');
    expect(garantieReq.request.params.get('pageSize')).toBe('1');
    garantieReq.flush({ items: [], totalCount: 0, page: 1, pageSize: 1 });

    const echeanceReq = httpMock.expectOne(r => r.url === `${API_BASE_URL}/Echeance`);
    expect(echeanceReq.request.params.get('contratId')).toBe('1');
    expect(echeanceReq.request.params.get('pageSize')).toBe('100');
    echeanceReq.flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });

    const retenueReq = httpMock.expectOne(r => r.url === `${API_BASE_URL}/RetenueMensuelle`);
    expect(retenueReq.request.params.get('contratId')).toBe('1');
    expect(retenueReq.request.params.get('pageSize')).toBe('100');
    retenueReq.flush({ items: [], totalCount: 0, page: 1, pageSize: 100 });

    expect(component.expandedContratId()).toBe(1);
  });
});
