import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, of } from 'rxjs';
import { DemandeService } from '../../core/services/demande.service';
import { AgentService } from '../../core/services/agent.service';
import { SocieteService } from '../../core/services/societe.service';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss'
})
export class HomePageComponent implements OnInit {
  private demandeService = inject(DemandeService);
  private agentService = inject(AgentService);
  private societeService = inject(SocieteService);

  demandesCount = signal<number | null>(null);
  agentsCount = signal<number | null>(null);
  societesCount = signal<number | null>(null);
  enCoursCount = signal<number | null>(null);

  ngOnInit(): void {
    // pageSize au maximum autorisé par l'API (100) pour ce tableau de bord: le total exact
    // vient de totalCount, mais le compte "en cours" est calculé sur les items reçus - au-delà
    // de 100 demandes, il sous-estimerait légèrement ce chiffre (limitation acceptée pour un
    // indicateur de tableau de bord, pas pour une donnée métier).
    this.demandeService.getAll({ pageSize: 100 })
      .pipe(catchError(() => of({ items: [], totalCount: 0, page: 1, pageSize: 100 })))
      .subscribe(result => {
        this.demandesCount.set(result.totalCount);
        this.enCoursCount.set(result.items.filter(d => ![8, 9, 10, 11].includes(d.statutCourant)).length);
      });
    this.agentService.getAll({ pageSize: 1 }).pipe(catchError(() => of({ items: [], totalCount: 0, page: 1, pageSize: 1 })))
      .subscribe(result => this.agentsCount.set(result.totalCount));
    this.societeService.getAll({ pageSize: 1 }).pipe(catchError(() => of({ items: [], totalCount: 0, page: 1, pageSize: 1 })))
      .subscribe(result => this.societesCount.set(result.totalCount));
  }
}
