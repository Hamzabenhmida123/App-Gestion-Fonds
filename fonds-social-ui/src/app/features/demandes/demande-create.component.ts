import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { DemandeService } from '../../core/services/demande.service';
import { AgentService } from '../../core/services/agent.service';
import { TypeDePretService } from '../../core/services/type-de-pret.service';
import { Agent, TypeDePret } from '../../core/models/referentiel.model';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-demande-create',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './demande-create.component.html'
})
export class DemandeCreateComponent implements OnInit {
  private service = inject(DemandeService);
  private agentService = inject(AgentService);
  private typeService = inject(TypeDePretService);
  private notifications = inject(NotificationService);
  private router = inject(Router);

  agents = signal<Agent[]>([]);
  types = signal<TypeDePret[]>([]);
  submitting = signal(false);
  formError = signal<string | null>(null);

  private fb = inject(FormBuilder);
  form = this.fb.nonNullable.group({
    agentId: [0, [Validators.required, Validators.min(1)]],
    typeDePretId: [0, [Validators.required, Validators.min(1)]],
    dateDepot: [new Date().toISOString().substring(0, 10), Validators.required],
    montantDemande: [0, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    // pageSize au maximum autorisé: ces sélecteurs doivent couvrir l'ensemble des agents/types
    // disponibles pour créer une demande, pas seulement la première page de la liste paginée.
    this.agentService.getAll({ pageSize: 100 }).subscribe(result => this.agents.set(result.items));
    this.typeService.getAll({ pageSize: 100 }).subscribe(result => this.types.set(result.items));
  }

  selectedType(): TypeDePret | undefined {
    return this.types().find(t => t.id === this.form.getRawValue().typeDePretId);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const raw = this.form.getRawValue();
    this.formError.set(null);
    this.submitting.set(true);
    this.service.create({
      agentId: raw.agentId,
      typeDePretId: raw.typeDePretId,
      dateDepot: new Date(raw.dateDepot).toISOString(),
      montantDemande: raw.montantDemande
    }).subscribe({
      next: created => {
        this.submitting.set(false);
        this.notifications.success(`Demande ${created.numeroDossier} créée.`);
        this.router.navigate(['/demandes', created.id]);
      },
      error: err => {
        this.submitting.set(false);
        this.formError.set(err.message);
      }
    });
  }
}
