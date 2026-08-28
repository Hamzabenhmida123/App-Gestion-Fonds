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
    this.demandeService.getAll().pipe(catchError(() => of([]))).subscribe(list => {
      this.demandesCount.set(list.length);
      this.enCoursCount.set(list.filter(d => ![8, 9, 10, 11].includes(d.statutCourant)).length);
    });
    this.agentService.getAll().pipe(catchError(() => of([]))).subscribe(list => this.agentsCount.set(list.length));
    this.societeService.getAll().pipe(catchError(() => of([]))).subscribe(list => this.societesCount.set(list.length));
  }
}
