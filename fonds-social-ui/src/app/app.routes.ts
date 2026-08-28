import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./features/home/home-page.component').then(m => m.HomePageComponent) },
  { path: 'societes', loadComponent: () => import('./features/societes/societes-page.component').then(m => m.SocietesPageComponent) },
  { path: 'agents', loadComponent: () => import('./features/agents/agents-page.component').then(m => m.AgentsPageComponent) },
  { path: 'types-de-pret', loadComponent: () => import('./features/types-de-pret/types-de-pret-page.component').then(m => m.TypesDePretPageComponent) },
  { path: 'demandes', loadComponent: () => import('./features/demandes/demande-list.component').then(m => m.DemandeListComponent) },
  { path: 'demandes/nouvelle', loadComponent: () => import('./features/demandes/demande-create.component').then(m => m.DemandeCreateComponent) },
  { path: 'demandes/:id', loadComponent: () => import('./features/demandes/demande-detail.component').then(m => m.DemandeDetailComponent) },
  { path: 'comite', loadComponent: () => import('./features/comite/comite-page.component').then(m => m.ComitePageComponent) },
  { path: 'finance', loadComponent: () => import('./features/finance/finance-page.component').then(m => m.FinancePageComponent) },
  { path: '**', redirectTo: '' }
];
