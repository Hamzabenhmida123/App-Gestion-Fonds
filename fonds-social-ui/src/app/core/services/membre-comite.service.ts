import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { CreateMembreComite, MembreComite } from '../models/comite.model';

@Injectable({ providedIn: 'root' })
export class MembreComiteService extends CrudService<MembreComite, CreateMembreComite> {
  protected endpoint = 'MembreComite';
}
