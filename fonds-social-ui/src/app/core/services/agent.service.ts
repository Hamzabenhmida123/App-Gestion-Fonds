import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { Agent, CreateAgent } from '../models/referentiel.model';

@Injectable({ providedIn: 'root' })
export class AgentService extends CrudService<Agent, CreateAgent> {
  protected endpoint = 'Agent';
}
