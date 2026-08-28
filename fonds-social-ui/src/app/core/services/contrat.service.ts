import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { Contrat, CreateContrat } from '../models/contrat.model';

@Injectable({ providedIn: 'root' })
export class ContratService extends CrudService<Contrat, CreateContrat> {
  protected endpoint = 'Contrat';
}
