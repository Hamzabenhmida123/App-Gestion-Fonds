import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SlicePipe } from '@angular/common';
import { ContratService } from '../../core/services/contrat.service';
import { GarantieService } from '../../core/services/garantie.service';
import { EcheanceService } from '../../core/services/echeance.service';
import { RetenueMensuelleService } from '../../core/services/retenue-mensuelle.service';
import { BudgetFondsService } from '../../core/services/budget-fonds.service';
import { DecisionService } from '../../core/services/decision.service';
import { AgentService } from '../../core/services/agent.service';
import { SocieteService } from '../../core/services/societe.service';
import { Contrat, Echeance, Garantie, RetenueMensuelle } from '../../core/models/contrat.model';
import { BudgetFonds } from '../../core/models/budget.model';
import { Decision } from '../../core/models/demande.model';
import { Agent, Societe } from '../../core/models/referentiel.model';
import {
  CATEGORIE_BUDGET_LABELS, CategorieBudget, STATUT_ECHEANCE_LABELS, STATUT_RETENUE_LABELS,
  StatutEcheance, StatutRetenue, enumOptions
} from '../../core/models/enums';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-finance-page',
  standalone: true,
  imports: [FormsModule, SlicePipe],
  templateUrl: './finance-page.component.html'
})
export class FinancePageComponent implements OnInit {
  private contratService = inject(ContratService);
  private garantieService = inject(GarantieService);
  private echeanceService = inject(EcheanceService);
  private retenueService = inject(RetenueMensuelleService);
  private budgetService = inject(BudgetFondsService);
  private decisionService = inject(DecisionService);
  private agentService = inject(AgentService);
  private societeService = inject(SocieteService);
  private notifications = inject(NotificationService);

  tab = signal<'contrats' | 'budgets'>('contrats');

  contrats = signal<Contrat[]>([]);
  decisions = signal<Decision[]>([]);
  agents = signal<Agent[]>([]);
  societes = signal<Societe[]>([]);

  contratEditingId = signal<number | null>(null);
  contratForm = {
    decisionId: 0, dateSignature: '', montantPrincipal: 0, fraisGestion: 0,
    montantTotal: 0, dureeMois: 12, datePremiereEcheance: ''
  };

  expandedContratId = signal<number | null>(null);
  garantie = signal<Garantie | null>(null);
  garantieForm = {
    assuranceVieSouscrite: false, dateAssuranceVie: '', referencePoliceAssurance: '',
    traiteSigneeLegalisee: false, dateTraite: '',
    engagementRemboursementSigne: false, dateEngagement: '',
    autorisationRetenueSalaireSignee: false, dateAutorisationRetenue: ''
  };

  echeances = signal<Echeance[]>([]);
  statutEcheanceOptions = enumOptions(STATUT_ECHEANCE_LABELS);
  statutEcheanceLabels = STATUT_ECHEANCE_LABELS;
  newEcheance = {
    numeroEcheance: 1, date: '', capitalRestantDu: 0, mensualite: 0,
    capitalAmorti: 0, fraisGestion: 0, soldeRestant: 0, statut: StatutEcheance.APayer
  };

  retenues = signal<RetenueMensuelle[]>([]);
  statutRetenueOptions = enumOptions(STATUT_RETENUE_LABELS);
  statutRetenueLabels = STATUT_RETENUE_LABELS;
  newRetenue = { mois: '', agentId: 0, montantARetenir: 0, statut: StatutRetenue.Prevue };

  budgets = signal<BudgetFonds[]>([]);
  budgetEditingId = signal<number | null>(null);
  categorieOptions = enumOptions(CATEGORIE_BUDGET_LABELS);
  categorieLabels = CATEGORIE_BUDGET_LABELS;
  budgetForm = {
    societeId: 0, exercice: new Date().getFullYear(), categorie: CategorieBudget.Logement,
    ressources: 0, emplois: 0, solde: 0
  };

  ngOnInit(): void {
    this.loadContrats();
    this.loadBudgets();
    this.decisionService.getAll().subscribe(list => this.decisions.set(list));
    this.agentService.getAll().subscribe(list => this.agents.set(list));
    this.societeService.getAll().subscribe(list => this.societes.set(list));
  }

  agentName(id: number): string {
    const a = this.agents().find(x => x.id === id);
    return a ? `${a.prenom} ${a.nom}` : `#${id}`;
  }

  societeName(id: number): string {
    return this.societes().find(s => s.id === id)?.raisonSociale ?? `#${id}`;
  }

  decisionLabel(id: number): string {
    const d = this.decisions().find(x => x.id === id);
    return d ? `Décision #${d.id} (demande #${d.demandeId})` : `#${id}`;
  }

  loadContrats(): void {
    this.contratService.getAll().subscribe(list => this.contrats.set(list));
  }

  startCreateContrat(): void {
    this.contratEditingId.set(0);
    this.contratForm = { decisionId: 0, dateSignature: '', montantPrincipal: 0, fraisGestion: 0, montantTotal: 0, dureeMois: 12, datePremiereEcheance: '' };
  }

  startEditContrat(c: Contrat): void {
    this.contratEditingId.set(c.id);
    this.contratForm = {
      decisionId: c.decisionId,
      dateSignature: c.dateSignature.substring(0, 10),
      montantPrincipal: c.montantPrincipal,
      fraisGestion: c.fraisGestion,
      montantTotal: c.montantTotal,
      dureeMois: c.dureeMois,
      datePremiereEcheance: c.datePremiereEcheance.substring(0, 10)
    };
  }

  cancelContrat(): void {
    this.contratEditingId.set(null);
  }

  saveContrat(): void {
    const id = this.contratEditingId();
    const dto = {
      ...this.contratForm,
      dateSignature: new Date(this.contratForm.dateSignature).toISOString(),
      datePremiereEcheance: new Date(this.contratForm.datePremiereEcheance).toISOString()
    };
    const done = () => {
      this.notifications.success(id ? 'Contrat mis à jour.' : 'Contrat créé.');
      this.contratEditingId.set(null);
      this.loadContrats();
    };
    if (id) {
      this.contratService.update(id, dto).subscribe({ next: done });
    } else {
      this.contratService.create(dto).subscribe({ next: done });
    }
  }

  removeContrat(c: Contrat): void {
    if (!confirm('Supprimer ce contrat ?')) return;
    this.contratService.delete(c.id).subscribe(() => { this.notifications.success('Contrat supprimé.'); this.loadContrats(); });
  }

  toggleContrat(c: Contrat): void {
    if (this.expandedContratId() === c.id) {
      this.expandedContratId.set(null);
      return;
    }
    this.expandedContratId.set(c.id);
    this.loadGarantie(c.id);
    this.loadEcheances(c.id);
    this.loadRetenues(c.id);
  }

  private loadGarantie(contratId: number): void {
    this.garantieService.getAll().subscribe(list => {
      const g = list.find(x => x.contratId === contratId) ?? null;
      this.garantie.set(g);
      this.garantieForm = g ? {
        assuranceVieSouscrite: g.assuranceVieSouscrite,
        dateAssuranceVie: g.dateAssuranceVie?.substring(0, 10) ?? '',
        referencePoliceAssurance: g.referencePoliceAssurance ?? '',
        traiteSigneeLegalisee: g.traiteSigneeLegalisee,
        dateTraite: g.dateTraite?.substring(0, 10) ?? '',
        engagementRemboursementSigne: g.engagementRemboursementSigne,
        dateEngagement: g.dateEngagement?.substring(0, 10) ?? '',
        autorisationRetenueSalaireSignee: g.autorisationRetenueSalaireSignee,
        dateAutorisationRetenue: g.dateAutorisationRetenue?.substring(0, 10) ?? ''
      } : {
        assuranceVieSouscrite: false, dateAssuranceVie: '', referencePoliceAssurance: '',
        traiteSigneeLegalisee: false, dateTraite: '',
        engagementRemboursementSigne: false, dateEngagement: '',
        autorisationRetenueSalaireSignee: false, dateAutorisationRetenue: ''
      };
    });
  }

  saveGarantie(): void {
    const contratId = this.expandedContratId();
    if (!contratId) return;
    const f = this.garantieForm;
    const dto = {
      contratId,
      assuranceVieSouscrite: f.assuranceVieSouscrite,
      dateAssuranceVie: f.dateAssuranceVie ? new Date(f.dateAssuranceVie).toISOString() : null,
      referencePoliceAssurance: f.referencePoliceAssurance || null,
      traiteSigneeLegalisee: f.traiteSigneeLegalisee,
      dateTraite: f.dateTraite ? new Date(f.dateTraite).toISOString() : null,
      engagementRemboursementSigne: f.engagementRemboursementSigne,
      dateEngagement: f.dateEngagement ? new Date(f.dateEngagement).toISOString() : null,
      autorisationRetenueSalaireSignee: f.autorisationRetenueSalaireSignee,
      dateAutorisationRetenue: f.dateAutorisationRetenue ? new Date(f.dateAutorisationRetenue).toISOString() : null
    };
    const existing = this.garantie();
    const done = () => { this.notifications.success('Garantie enregistrée.'); this.loadGarantie(contratId); };
    if (existing) {
      this.garantieService.update(existing.id, dto).subscribe({ next: done });
    } else {
      this.garantieService.create(dto).subscribe({ next: done });
    }
  }

  private loadEcheances(contratId: number): void {
    this.echeanceService.getAll().subscribe(list => {
      this.echeances.set(list.filter(e => e.contratId === contratId).sort((a, b) => a.numeroEcheance - b.numeroEcheance));
    });
  }

  addEcheance(): void {
    const contratId = this.expandedContratId();
    if (!contratId) return;
    this.echeanceService.create({
      contratId,
      ...this.newEcheance,
      date: this.newEcheance.date ? new Date(this.newEcheance.date).toISOString() : new Date().toISOString()
    }).subscribe(() => {
      this.newEcheance = { numeroEcheance: this.newEcheance.numeroEcheance + 1, date: '', capitalRestantDu: 0, mensualite: 0, capitalAmorti: 0, fraisGestion: 0, soldeRestant: 0, statut: StatutEcheance.APayer };
      this.loadEcheances(contratId);
    });
  }

  private loadRetenues(contratId: number): void {
    this.retenueService.getAll().subscribe(list => {
      this.retenues.set(list.filter(r => r.contratId === contratId));
    });
  }

  addRetenue(): void {
    const contratId = this.expandedContratId();
    if (!contratId || !this.newRetenue.agentId) return;
    this.retenueService.create({
      mois: this.newRetenue.mois ? new Date(this.newRetenue.mois).toISOString() : new Date().toISOString(),
      agentId: this.newRetenue.agentId,
      contratId,
      montantARetenir: this.newRetenue.montantARetenir,
      statut: this.newRetenue.statut
    }).subscribe(() => {
      this.newRetenue = { mois: '', agentId: 0, montantARetenir: 0, statut: StatutRetenue.Prevue };
      this.loadRetenues(contratId);
    });
  }

  loadBudgets(): void {
    this.budgetService.getAll().subscribe(list => this.budgets.set(list));
  }

  startCreateBudget(): void {
    this.budgetEditingId.set(0);
    this.budgetForm = { societeId: this.societes()[0]?.id ?? 0, exercice: new Date().getFullYear(), categorie: CategorieBudget.Logement, ressources: 0, emplois: 0, solde: 0 };
  }

  startEditBudget(b: BudgetFonds): void {
    this.budgetEditingId.set(b.id);
    this.budgetForm = { societeId: b.societeId, exercice: b.exercice, categorie: b.categorie, ressources: b.ressources, emplois: b.emplois, solde: b.solde };
  }

  cancelBudget(): void {
    this.budgetEditingId.set(null);
  }

  saveBudget(): void {
    const id = this.budgetEditingId();
    const dto = { ...this.budgetForm };
    const done = () => {
      this.notifications.success(id ? 'Budget mis à jour.' : 'Budget créé.');
      this.budgetEditingId.set(null);
      this.loadBudgets();
    };
    if (id) {
      this.budgetService.update(id, dto).subscribe({ next: done });
    } else {
      this.budgetService.create(dto).subscribe({ next: done });
    }
  }

  removeBudget(b: BudgetFonds): void {
    if (!confirm('Supprimer ce budget ?')) return;
    this.budgetService.delete(b.id).subscribe(() => { this.notifications.success('Budget supprimé.'); this.loadBudgets(); });
  }
}
