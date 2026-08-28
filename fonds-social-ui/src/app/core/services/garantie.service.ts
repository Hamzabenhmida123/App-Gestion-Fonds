import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { CreateGarantie, Garantie } from '../models/contrat.model';

@Injectable({ providedIn: 'root' })
export class GarantieService extends CrudService<Garantie, CreateGarantie> {
  protected endpoint = 'Garantie';
}
