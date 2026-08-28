import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { CreateEcheance, Echeance } from '../models/contrat.model';

@Injectable({ providedIn: 'root' })
export class EcheanceService extends CrudService<Echeance, CreateEcheance> {
  protected endpoint = 'Echeance';
}
