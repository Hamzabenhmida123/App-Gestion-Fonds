import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { CreateParticipationSeance, ParticipationSeance } from '../models/comite.model';

@Injectable({ providedIn: 'root' })
export class ParticipationSeanceService extends CrudService<ParticipationSeance, CreateParticipationSeance> {
  protected endpoint = 'ParticipationSeance';
}
