import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { SocieteService } from '../../core/services/societe.service';
import { Societe } from '../../core/models/referentiel.model';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-societes-page',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './societes-page.component.html'
})
export class SocietesPageComponent implements OnInit {
  private service = inject(SocieteService);
  private fb = inject(FormBuilder);
  private notifications = inject(NotificationService);

  societes = signal<Societe[]>([]);
  loading = signal(false);
  editingId = signal<number | null>(null);
  formError = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    code: ['', Validators.required],
    raisonSociale: ['', Validators.required],
    referentielReglesGestion: ['']
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: list => { this.societes.set(list); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  startCreate(): void {
    this.editingId.set(0);
    this.formError.set(null);
    this.form.reset({ code: '', raisonSociale: '', referentielReglesGestion: '' });
  }

  startEdit(s: Societe): void {
    this.editingId.set(s.id);
    this.formError.set(null);
    this.form.reset({
      code: s.code,
      raisonSociale: s.raisonSociale,
      referentielReglesGestion: s.referentielReglesGestion ?? ''
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
      this.notifications.success(id ? 'Société mise à jour.' : 'Société créée.');
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

  remove(s: Societe): void {
    if (!confirm(`Supprimer la société "${s.raisonSociale}" ?`)) return;
    this.service.delete(s.id).subscribe({
      next: () => { this.notifications.success('Société supprimée.'); this.load(); }
    });
  }
}
