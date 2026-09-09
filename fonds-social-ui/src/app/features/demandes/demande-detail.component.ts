import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SlicePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { API_BASE_URL } from '../../core/config/app-config';
import { DemandeService } from '../../core/services/demande.service';
import { AgentService } from '../../core/services/agent.service';
import { TypeDePretService } from '../../core/services/type-de-pret.service';
import { Demande } from '../../core/models/demande.model';
import { Agent, TypeDePret } from '../../core/models/referentiel.model';
import {
  SENS_DECISION_COLORS, SENS_DECISION_LABELS,
  STATUT_DEMANDE_COLORS, STATUT_DEMANDE_LABELS, STATUT_DEMANDE_TRANSITIONS, StatutDemande,
  STATUT_VERIFICATION_COLORS, STATUT_VERIFICATION_LABELS, StatutVerification
} from '../../core/models/enums';
import { StatusBadgeComponent } from '../../shared/ui/status-badge/status-badge.component';
import { NotificationService } from '../../core/services/notification.service';
import { CurrentUserService } from '../../core/services/current-user.service';

@Component({
  selector: 'app-demande-detail',
  standalone: true,
  imports: [FormsModule, RouterLink, StatusBadgeComponent, SlicePipe],
  templateUrl: './demande-detail.component.html'
})
export class DemandeDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private service = inject(DemandeService);
  private agentService = inject(AgentService);
  private typeService = inject(TypeDePretService);
  private notifications = inject(NotificationService);
  private currentUser = inject(CurrentUserService);

  demande = signal<Demande | null>(null);
  agent = signal<Agent | null>(null);
  type = signal<TypeDePret | null>(null);
  loading = signal(false);

  // Auteur partagé, saisi une fois et persisté (voir CurrentUserService) plutôt que
  // répété/retapé pour chaque action, ou saisi via un prompt() natif pour les pièces.
  auteurInput = this.currentUser.auteur();
  auteurError = signal<string | null>(null);

  cloturerCommentaire = '';
  cloturerError = signal<string | null>(null);

  transitionTarget: StatutDemande | null = null;
  transitionCommentaire = '';
  transitionError = signal<string | null>(null);

  uploadTypePiece = '';
  uploadFile: File | null = null;
  uploadError = signal<string | null>(null);
  uploadResult = signal<string[] | null>(null);

  statutLabels = STATUT_DEMANDE_LABELS;
  statutColors = STATUT_DEMANDE_COLORS;
  verificationLabels = STATUT_VERIFICATION_LABELS;
  verificationColors = STATUT_VERIFICATION_COLORS;
  sensLabels = SENS_DECISION_LABELS;
  sensColors = SENS_DECISION_COLORS;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.load(id);
  }

  private load(id: number): void {
    this.loading.set(true);
    this.service.getById(id).subscribe({
      next: d => {
        this.demande.set(d);
        this.loading.set(false);
        this.agentService.getById(d.agentId).subscribe(a => this.agent.set(a));
        this.typeService.getById(d.typeDePretId).subscribe(t => this.type.set(t));
      },
      error: () => this.loading.set(false)
    });
  }

  reload(): void {
    const d = this.demande();
    if (d) this.load(d.id);
  }

  availableTransitions(): StatutDemande[] {
    const d = this.demande();
    if (!d) return [];
    return STATUT_DEMANDE_TRANSITIONS[d.statutCourant] ?? [];
  }

  /** Valide l'auteur partagé, le persiste pour les prochaines actions, ou signale l'erreur. */
  private resolveAuteur(): string | null {
    const value = this.auteurInput.trim();
    if (!value) {
      this.auteurError.set('Veuillez renseigner votre nom (auteur de l\'action) avant de continuer.');
      return null;
    }
    this.auteurError.set(null);
    this.currentUser.setAuteur(value);
    return value;
  }

  submitTransition(): void {
    const d = this.demande();
    const auteur = this.resolveAuteur();
    if (!d || this.transitionTarget == null || !auteur) return;
    this.transitionError.set(null);
    this.service.transition(d.id, this.transitionTarget, auteur, this.transitionCommentaire.trim() || undefined)
      .subscribe({
        next: () => {
          this.notifications.success('Transition effectuée.');
          this.transitionTarget = null;
          this.transitionCommentaire = '';
          this.reload();
        },
        error: err => this.transitionError.set(err.message)
      });
  }

  submitCloturerDepot(): void {
    const d = this.demande();
    const auteur = this.resolveAuteur();
    if (!d || !auteur) return;
    this.cloturerError.set(null);
    this.service.cloturerDepot(d.id, auteur, this.cloturerCommentaire.trim() || undefined)
      .subscribe({
        next: () => {
          this.notifications.success('Dépôt clôturé, demande enregistrée.');
          this.cloturerCommentaire = '';
          this.reload();
        },
        error: err => this.cloturerError.set(err.message)
      });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.uploadFile = input.files?.[0] ?? null;
  }

  submitUpload(): void {
    const d = this.demande();
    if (!d || !this.uploadFile || !this.uploadTypePiece.trim()) return;
    this.uploadError.set(null);
    this.uploadResult.set(null);
    this.service.uploadPiece(d.id, this.uploadFile, this.uploadTypePiece.trim()).subscribe({
      next: res => {
        this.notifications.success('Pièce déposée.');
        this.uploadResult.set(res.missingRequired);
        this.uploadTypePiece = '';
        this.uploadFile = null;
        this.reload();
      },
      error: err => this.uploadError.set(err.message)
    });
  }

  pieceFileUrl(pieceId: number): string {
    return `${API_BASE_URL}/Demande/pieces/${pieceId}/file`;
  }

  changePieceStatus(pieceId: number, statut: StatutVerification): void {
    const auteur = this.resolveAuteur();
    if (!auteur) return;
    this.service.changePieceStatus(pieceId, statut, auteur).subscribe({
      next: () => { this.notifications.success('Statut de la pièce mis à jour.'); this.reload(); }
    });
  }
}
