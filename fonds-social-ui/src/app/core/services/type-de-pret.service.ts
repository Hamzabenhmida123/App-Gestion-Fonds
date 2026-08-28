import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { CreateTypeDePret, TypeDePret } from '../models/referentiel.model';

@Injectable({ providedIn: 'root' })
export class TypeDePretService extends CrudService<TypeDePret, CreateTypeDePret> {
  protected endpoint = 'TypeDePret';
}
