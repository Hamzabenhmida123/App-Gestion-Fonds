import { CategorieBudget } from './enums';

export interface BudgetFonds {
  id: number;
  societeId: number;
  exercice: number;
  categorie: CategorieBudget;
  ressources: number;
  emplois: number;
  solde: number;
}

export type CreateBudgetFonds = Omit<BudgetFonds, 'id'>;
