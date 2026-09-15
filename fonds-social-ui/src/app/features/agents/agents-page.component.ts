import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { SlicePipe } from '@angular/common';
import { AgentService } from '../../core/services/agent.service';
import { SocieteService } from '../../core/services/societe.service';
import { Agent, Societe } from '../../core/models/referentiel.model';
import { NotificationService } from '../../core/services/notification.service';
import { SITUATION_FAMILIALE_LABELS, SituationFamiliale, enumOptions } from '../../core/models/enums';

@Component({
  selector: 'app-agents-page',
  standalone: true,
  imports: [ReactiveFormsModule, SlicePipe],
  templateUrl: './agents-page.component.html'
})
export class AgentsPageComponent implements OnInit {
  private service = inject(AgentService);
  private societeService = inject(SocieteService);
  private fb = inject(FormBuilder);
  private notifications = inject(NotificationService);

  agents = signal<Agent[]>([]);
  societes = signal<Societe[]>([]);
  loading = signal(false);
  editingId = signal<number | null>(null);
  formError = signal<string | null>(null);

  readonly pageSize = 20;
  page = signal(1);
  totalCount = signal(0);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  situationFamilialeOptions = enumOptions(SITUATION_FAMILIALE_LABELS);

  form = this.fb.group({
    matricule: this.fb.nonNullable.control('', Validators.required),
    nom: this.fb.nonNullable.control('', Validators.required),
    prenom: this.fb.nonNullable.control('', Validators.required),
    cin: this.fb.nonNullable.control('', Validators.required),
    identifiantUnique: this.fb.nonNullable.control('', Validators.required),
    grade: this.fb.nonNullable.control(''),
    fonction: this.fb.nonNullable.control(''),
    adresse: this.fb.nonNullable.control(''),
    telephone: this.fb.nonNullable.control(''),
    dateTitularisation: this.fb.nonNullable.control(''),
    situationFamiliale: this.fb.control<SituationFamiliale | null>(null),
    nombreEnfantsACharge: this.fb.nonNullable.control(0, [Validators.required, Validators.min(0)]),
    salaireMensuel: this.fb.nonNullable.control(0, [Validators.min(0)]),
    societeId: this.fb.nonNullable.control(0, [Validators.required, Validators.min(1)])
  });

  societeName(id: number): string {
    return this.societes().find(s => s.id === id)?.raisonSociale ?? `#${id}`;
  }

  situationLabel(s: SituationFamiliale | null | undefined): string {
    return s == null ? '—' : SITUATION_FAMILIALE_LABELS[s];
  }

  ngOnInit(): void {
    // pageSize au maximum autorisé: ce sélecteur doit couvrir l'ensemble des sociétés
    // disponibles pour le formulaire, pas seulement la première page de la liste paginée.
    this.societeService.getAll({ pageSize: 100 }).subscribe(result => this.societes.set(result.items));
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll({ page: this.page(), pageSize: this.pageSize }).subscribe({
      next: result => {
        this.agents.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  previousPage(): void {
    if (this.page() <= 1) return;
    this.page.update(p => p - 1);
    this.load();
  }

  nextPage(): void {
    if (this.page() >= this.totalPages()) return;
    this.page.update(p => p + 1);
    this.load();
  }

  startCreate(): void {
    this.editingId.set(0);
    this.formError.set(null);
    this.form.reset({
      matricule: '', nom: '', prenom: '', cin: '', identifiantUnique: '',
      grade: '', fonction: '', adresse: '', telephone: '', dateTitularisation: '',
      situationFamiliale: null,
      nombreEnfantsACharge: 0, salaireMensuel: 0, societeId: this.societes()[0]?.id ?? 0
    });
  }

  startEdit(a: Agent): void {
    this.editingId.set(a.id);
    this.formError.set(null);
    this.form.reset({
      matricule: a.matricule,
      nom: a.nom,
      prenom: a.prenom,
      cin: a.cin,
      identifiantUnique: a.identifiantUnique,
      grade: a.grade ?? '',
      fonction: a.fonction ?? '',
      adresse: a.adresse ?? '',
      telephone: a.telephone ?? '',
      dateTitularisation: a.dateTitularisation ? a.dateTitularisation.substring(0, 10) : '',
      situationFamiliale: a.situationFamiliale ?? null,
      nombreEnfantsACharge: a.nombreEnfantsACharge,
      salaireMensuel: a.salaireMensuel ?? 0,
      societeId: a.societeId
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
    const raw = this.form.getRawValue();
    const dto = {
      ...raw,
      dateTitularisation: raw.dateTitularisation ? new Date(raw.dateTitularisation).toISOString() : null
    };
    this.formError.set(null);

    const done = () => {
      this.notifications.success(id ? 'Agent mis à jour.' : 'Agent créé.');
      this.editingId.set(null);
      this.load();
    };
    const fail = (err: Error) => this.formError.set(err.message);

    if (id) {
      this.service.update(id, dto).subscribe({ next: done, error: fail });
    } else {
      this.service.create(dto).subscribe({ next: done, error: fail });
    }
  }

  remove(a: Agent): void {
    if (!confirm(`Supprimer l'agent "${a.prenom} ${a.nom}" ?`)) return;
    this.service.delete(a.id).subscribe({
      next: () => { this.notifications.success('Agent supprimé.'); this.load(); }
    });
  }
}
