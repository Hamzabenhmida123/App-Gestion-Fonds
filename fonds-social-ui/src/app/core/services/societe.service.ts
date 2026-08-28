import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { CreateSociete, Societe } from '../models/referentiel.model';

@Injectable({ providedIn: 'root' })
export class SocieteService extends CrudService<Societe, CreateSociete> {
  protected endpoint = 'Societe';
}
