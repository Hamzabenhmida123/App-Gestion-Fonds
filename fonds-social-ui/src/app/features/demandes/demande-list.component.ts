import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SlicePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DemandeService } from '../../core/services/demande.service';
import { AgentService } from '../../core/services/agent.service';
import { TypeDePretService } from '../../core/services/type-de-pret.service';
import { Demande } from '../../core/models/demande.model';
import { Agent, TypeDePret } from '../../core/models/referentiel.model';
import { STATUT_DEMANDE_COLORS, STATUT_DEMANDE_LABELS, StatutDemande, enumOptions } from '../../core/models/enums';
import { StatusBadgeComponent } from '../../shared/ui/status-badge/status-badge.component';

@Component({
  selector: 'app-demande-list',
  standalone: true,
  imports: [FormsModule, RouterLink, StatusBadgeComponent, SlicePipe],
  templateUrl: './demande-list.component.html'
})
export class DemandeListComponent implements OnInit {
  private service = inject(DemandeService);
  private agentService = inject(AgentService);
  private typeService = inject(TypeDePretService);

  demandes = signal<Demande[]>([]);
  agents = signal<Agent[]>([]);
  types = signal<TypeDePret[]>([]);
  loading = signal(false);

  readonly pageSize = 20;
  page = signal(1);
  totalCount = signal(0);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  statutOptions = enumOptions(STATUT_DEMANDE_LABELS);

  filterAgentId: number | null = null;
  filterStatut: StatutDemande | null = null;
  filterTypeId: number | null = null;
  filterFrom = '';
  filterTo = '';

  ngOnInit(): void {
    this.agentService.getAll().subscribe(list => this.agents.set(list));
    this.typeService.getAll().subscribe(list => this.types.set(list));
    this.load();
  }

  statutLabel(s: StatutDemande): string {
    return STATUT_DEMANDE_LABELS[s];
  }

  statutColor(s: StatutDemande): string {
    return STATUT_DEMANDE_COLORS[s];
  }

  agentName(id: number): string {
    const a = this.agents().find(x => x.id === id);
    return a ? `${a.prenom} ${a.nom}` : `#${id}`;
  }

  typeName(id: number): string {
    return this.types().find(t => t.id === id)?.libelle ?? `#${id}`;
  }

  /** Rechargement suite à un changement de filtre: on repart de la page 1. */
  applyFilters(): void {
    this.page.set(1);
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll({
      agentId: this.filterAgentId ?? undefined,
      statut: this.filterStatut ?? undefined,
      typeDePretId: this.filterTypeId ?? undefined,
      from: this.filterFrom || undefined,
      to: this.filterTo || undefined,
      page: this.page(),
      pageSize: this.pageSize
    }).subscribe({
      next: result => {
        this.demandes.set(result.items);
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

  resetFilters(): void {
    this.filterAgentId = null;
    this.filterStatut = null;
    this.filterTypeId = null;
    this.filterFrom = '';
    this.filterTo = '';
    this.applyFilters();
  }
}
