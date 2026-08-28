import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { BudgetFonds, CreateBudgetFonds } from '../models/budget.model';

@Injectable({ providedIn: 'root' })
export class BudgetFondsService extends CrudService<BudgetFonds, CreateBudgetFonds> {
  protected endpoint = 'BudgetFonds';
}
