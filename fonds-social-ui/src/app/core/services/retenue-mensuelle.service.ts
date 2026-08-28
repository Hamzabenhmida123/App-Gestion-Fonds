import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { CreateRetenueMensuelle, RetenueMensuelle } from '../models/contrat.model';

@Injectable({ providedIn: 'root' })
export class RetenueMensuelleService extends CrudService<RetenueMensuelle, CreateRetenueMensuelle> {
  protected endpoint = 'RetenueMensuelle';
}
