import { Injectable } from '@angular/core';
import { CrudService } from './crud.service';
import { CreatePieceJustificativeRequise, PieceJustificativeRequise } from '../models/referentiel.model';

@Injectable({ providedIn: 'root' })
export class PieceJustificativeRequiseService extends CrudService<PieceJustificativeRequise, CreatePieceJustificativeRequise> {
  protected endpoint = 'PieceJustificativeRequise';
}
