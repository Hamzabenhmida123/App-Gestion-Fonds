import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { Decision } from '../models/demande.model';

export interface CreateDecision {
  demandeId: number;
  seanceComiteId: number;
  sensDecision: number;
  montantAccorde?: number | null;
  dateNotification?: string | null;
  datePeremption?: string | null;
}

@Injectable({ providedIn: 'root' })
export class DecisionService extends CrudService<Decision, CreateDecision> {
  protected endpoint = 'Decision';
}
