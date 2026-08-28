import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { CreateSeanceComite, SeanceComite } from '../models/comite.model';

@Injectable({ providedIn: 'root' })
export class SeanceComiteService extends CrudService<SeanceComite, CreateSeanceComite> {
  protected endpoint = 'SeanceComite';
}
