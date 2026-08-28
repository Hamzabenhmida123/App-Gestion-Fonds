import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TypeDePretService } from '../../core/services/type-de-pret.service';
import { PieceJustificativeRequiseService } from '../../core/services/piece-justificative-requise.service';
import { PieceJustificativeRequise, TypeDePret } from '../../core/models/referentiel.model';
import { CATEGORIE_BUDGET_LABELS, CategorieBudget, MODE_CALCUL_FRAIS_LABELS, ModeCalculFraisGestion, enumOptions } from '../../core/models/enums';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-types-de-pret-page',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule],
  templateUrl: './types-de-pret-page.component.html'
})
export class TypesDePretPageComponent implements OnInit {
  private service = inject(TypeDePretService);
  private pieceService = inject(PieceJustificativeRequiseService);
  private fb = inject(FormBuilder);
  private notifications = inject(NotificationService);

  types = signal<TypeDePret[]>([]);
  loading = signal(false);
  editingId = signal<number | null>(null);
  formError = signal<string | null>(null);

  expandedTypeId = signal<number | null>(null);
  pieces = signal<PieceJustificativeRequise[]>([]);
  newPieceLabel = '';
  newPieceObligatoire = true;

  categorieOptions = enumOptions(CATEGORIE_BUDGET_LABELS);
  modeFraisOptions = enumOptions(MODE_CALCUL_FRAIS_LABELS);

  form = this.fb.nonNullable.group({
    code: ['', Validators.required],
    libelle: ['', Validators.required],
    categorie: [CategorieBudget.Logement, Validators.required],
    plafond: [0, [Validators.required, Validators.min(0)]],
    dureeMaxMois: [12, [Validators.required, Validators.min(1)]],
    franchiseMois: [0, [Validators.required, Validators.min(0)]],
    modeCalculFraisGestion: [ModeCalculFraisGestion.PourcentageConstant, Validators.required],
    tauxOuMontantFraisGestion: [0, [Validators.required, Validators.min(0)]],
    actif: [true]
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: list => { this.types.set(list); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  categorieLabel(c: number): string {
    return CATEGORIE_BUDGET_LABELS[c as CategorieBudget] ?? String(c);
  }

  startCreate(): void {
    this.editingId.set(0);
    this.formError.set(null);
    this.form.reset({
      code: '', libelle: '', categorie: CategorieBudget.Logement, plafond: 0,
      dureeMaxMois: 12, franchiseMois: 0, modeCalculFraisGestion: ModeCalculFraisGestion.PourcentageConstant,
      tauxOuMontantFraisGestion: 0, actif: true
    });
  }

  startEdit(t: TypeDePret): void {
    this.editingId.set(t.id);
    this.formError.set(null);
    this.form.reset({
      code: t.code, libelle: t.libelle, categorie: t.categorie, plafond: t.plafond,
      dureeMaxMois: t.dureeMaxMois, franchiseMois: t.franchiseMois,
      modeCalculFraisGestion: t.modeCalculFraisGestion, tauxOuMontantFraisGestion: t.tauxOuMontantFraisGestion,
      actif: t.actif
    });
  }

  cancel(): void {
    this.editingId.set(null);
    this.formError.set(null);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const id = this.editingId();
    const value = this.form.getRawValue();
    this.formError.set(null);

    const done = () => {
      this.notifications.success(id ? 'Type de prêt mis à jour.' : 'Type de prêt créé.');
      this.editingId.set(null);
      this.load();
    };
    const fail = (err: Error) => this.formError.set(err.message);

    if (id) {
      this.service.update(id, value).subscribe({ next: done, error: fail });
    } else {
      this.service.create(value).subscribe({ next: done, error: fail });
    }
  }

  remove(t: TypeDePret): void {
    if (!confirm(`Supprimer le type de prêt "${t.libelle}" ?`)) return;
    this.service.delete(t.id).subscribe({
      next: () => { this.notifications.success('Type de prêt supprimé.'); this.load(); }
    });
  }

  toggleRequiredDocs(t: TypeDePret): void {
    if (this.expandedTypeId() === t.id) {
      this.expandedTypeId.set(null);
      return;
    }
    this.expandedTypeId.set(t.id);
    this.loadPieces(t.id);
  }

  private loadPieces(typeDePretId: number): void {
    this.pieceService.getAll().subscribe(list => {
      this.pieces.set(list.filter(p => p.typeDePretId === typeDePretId));
    });
  }

  addPiece(): void {
    const typeId = this.expandedTypeId();
    if (!typeId || !this.newPieceLabel.trim()) return;
    this.pieceService.create({
      typeDePretId: typeId,
      libellePiece: this.newPieceLabel.trim(),
      obligatoire: this.newPieceObligatoire
    }).subscribe(() => {
      this.newPieceLabel = '';
      this.newPieceObligatoire = true;
      this.loadPieces(typeId);
    });
  }

  removePiece(p: PieceJustificativeRequise): void {
    if (!confirm(`Retirer la pièce "${p.libellePiece}" ?`)) return;
    this.pieceService.delete(p.id).subscribe(() => this.loadPieces(p.typeDePretId));
  }
}
