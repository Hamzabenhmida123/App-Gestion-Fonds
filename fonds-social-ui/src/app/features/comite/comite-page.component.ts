import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SlicePipe } from '@angular/common';
import { MembreComiteService } from '../../core/services/membre-comite.service';
import { SeanceComiteService } from '../../core/services/seance-comite.service';
import { ParticipationSeanceService } from '../../core/services/participation-seance.service';
import { DecisionService } from '../../core/services/decision.service';
import { DemandeService } from '../../core/services/demande.service';
import { MembreComite, ParticipationSeance, SeanceComite } from '../../core/models/comite.model';
import { Decision, Demande } from '../../core/models/demande.model';
import {
  SENS_DECISION_LABELS, STATUT_VISA_LABELS, SensDecision, StatutVisaSignature, enumOptions
} from '../../core/models/enums';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-comite-page',
  standalone: true,
  imports: [FormsModule, SlicePipe],
  templateUrl: './comite-page.component.html'
})
export class ComitePageComponent implements OnInit {
  private membreService = inject(MembreComiteService);
  private seanceService = inject(SeanceComiteService);
  private participationService = inject(ParticipationSeanceService);
  private decisionService = inject(DecisionService);
  private demandeService = inject(DemandeService);
  private notifications = inject(NotificationService);

  tab = signal<'membres' | 'seances'>('membres');

  membres = signal<MembreComite[]>([]);
  membreEditingId = signal<number | null>(null);
  membreForm = { nom: '', prenom: '', fonction: '', actif: true };

  seances = signal<SeanceComite[]>([]);
  seanceEditingId = signal<number | null>(null);
  seanceForm: { date: string; procesVerbal: string; statutVisaSignature: StatutVisaSignature } = {
    date: '', procesVerbal: '', statutVisaSignature: StatutVisaSignature.EnPreparation
  };
  statutVisaOptions = enumOptions(STATUT_VISA_LABELS);
  statutVisaLabels = STATUT_VISA_LABELS;

  expandedSeanceId = signal<number | null>(null);
  participations = signal<ParticipationSeance[]>([]);
  decisions = signal<Decision[]>([]);
  demandes = signal<Demande[]>([]);
  sensOptions = enumOptions(SENS_DECISION_LABELS);
  sensLabels = SENS_DECISION_LABELS;

  newParticipation = { membreComiteId: 0, aVise: false, dateVisa: '' };
  newDecision: { demandeId: number; sensDecision: SensDecision; montantAccorde: number | null } = {
    demandeId: 0, sensDecision: SensDecision.Favorable, montantAccorde: null
  };

  ngOnInit(): void {
    this.loadMembres();
    this.loadSeances();
    this.demandeService.getAll().subscribe(list => this.demandes.set(list));
  }

  membreName(id: number): string {
    const m = this.membres().find(x => x.id === id);
    return m ? `${m.prenom} ${m.nom}` : `#${id}`;
  }

  demandeLabel(id: number): string {
    const d = this.demandes().find(x => x.id === id);
    return d ? d.numeroDossier : `#${id}`;
  }

  loadMembres(): void {
    this.membreService.getAll().subscribe(list => this.membres.set(list));
  }

  startCreateMembre(): void {
    this.membreEditingId.set(0);
    this.membreForm = { nom: '', prenom: '', fonction: '', actif: true };
  }

  startEditMembre(m: MembreComite): void {
    this.membreEditingId.set(m.id);
    this.membreForm = { nom: m.nom, prenom: m.prenom, fonction: m.fonction ?? '', actif: m.actif };
  }

  cancelMembre(): void {
    this.membreEditingId.set(null);
  }

  saveMembre(): void {
    const id = this.membreEditingId();
    const dto = { ...this.membreForm };
    const done = () => {
      this.notifications.success(id ? 'Membre mis à jour.' : 'Membre créé.');
      this.membreEditingId.set(null);
      this.loadMembres();
    };
    if (id) {
      this.membreService.update(id, dto).subscribe({ next: done });
    } else {
      this.membreService.create(dto).subscribe({ next: done });
    }
  }

  removeMembre(m: MembreComite): void {
    if (!confirm(`Supprimer le membre "${m.prenom} ${m.nom}" ?`)) return;
    this.membreService.delete(m.id).subscribe(() => { this.notifications.success('Membre supprimé.'); this.loadMembres(); });
  }

  loadSeances(): void {
    this.seanceService.getAll().subscribe(list => this.seances.set(list));
  }

  startCreateSeance(): void {
    this.seanceEditingId.set(0);
    this.seanceForm = { date: '', procesVerbal: '', statutVisaSignature: StatutVisaSignature.EnPreparation };
  }

  startEditSeance(s: SeanceComite): void {
    this.seanceEditingId.set(s.id);
    this.seanceForm = {
      date: s.date.substring(0, 10),
      procesVerbal: s.procesVerbal ?? '',
      statutVisaSignature: s.statutVisaSignature
    };
  }

  cancelSeance(): void {
    this.seanceEditingId.set(null);
  }

  saveSeance(): void {
    const id = this.seanceEditingId();
    const dto = {
      date: this.seanceForm.date ? new Date(this.seanceForm.date).toISOString() : new Date().toISOString(),
      procesVerbal: this.seanceForm.procesVerbal,
      statutVisaSignature: this.seanceForm.statutVisaSignature
    };
    const done = () => {
      this.notifications.success(id ? 'Séance mise à jour.' : 'Séance créée.');
      this.seanceEditingId.set(null);
      this.loadSeances();
    };
    if (id) {
      this.seanceService.update(id, dto).subscribe({ next: done });
    } else {
      this.seanceService.create(dto).subscribe({ next: done });
    }
  }

  removeSeance(s: SeanceComite): void {
    if (!confirm('Supprimer cette séance ?')) return;
    this.seanceService.delete(s.id).subscribe(() => { this.notifications.success('Séance supprimée.'); this.loadSeances(); });
  }

  toggleSeance(s: SeanceComite): void {
    if (this.expandedSeanceId() === s.id) {
      this.expandedSeanceId.set(null);
      return;
    }
    this.expandedSeanceId.set(s.id);
    this.loadParticipations(s.id);
    this.loadDecisions(s.id);
  }

  private loadParticipations(seanceId: number): void {
    this.participationService.getAll().subscribe(list => {
      this.participations.set(list.filter(p => p.seanceComiteId === seanceId));
    });
  }

  private loadDecisions(seanceId: number): void {
    this.decisionService.getAll().subscribe(list => {
      this.decisions.set(list.filter(d => d.seanceComiteId === seanceId));
    });
  }

  addParticipation(): void {
    const seanceId = this.expandedSeanceId();
    if (!seanceId || !this.newParticipation.membreComiteId) return;
    this.participationService.create({
      seanceComiteId: seanceId,
      membreComiteId: this.newParticipation.membreComiteId,
      aVise: this.newParticipation.aVise,
      dateVisa: this.newParticipation.dateVisa ? new Date(this.newParticipation.dateVisa).toISOString() : null
    }).subscribe(() => {
      this.newParticipation = { membreComiteId: 0, aVise: false, dateVisa: '' };
      this.loadParticipations(seanceId);
    });
  }

  removeParticipation(p: ParticipationSeance): void {
    this.participationService.delete(p.id).subscribe(() => this.loadParticipations(p.seanceComiteId));
  }

  addDecision(): void {
    const seanceId = this.expandedSeanceId();
    if (!seanceId || !this.newDecision.demandeId) return;
    this.decisionService.create({
      demandeId: this.newDecision.demandeId,
      seanceComiteId: seanceId,
      sensDecision: this.newDecision.sensDecision,
      montantAccorde: this.newDecision.montantAccorde
    }).subscribe(() => {
      this.notifications.success('Décision enregistrée.');
      this.newDecision = { demandeId: 0, sensDecision: SensDecision.Favorable, montantAccorde: null };
      this.loadDecisions(seanceId);
    });
  }
}
