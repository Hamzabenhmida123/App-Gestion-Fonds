import { Component, inject } from '@angular/core';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  template: `
    <div class="toast-stack">
      @for (toast of notifications.toasts(); track toast.id) {
        <div class="toast" [class.toast-error]="toast.kind === 'error'" [class.toast-success]="toast.kind === 'success'">
          <span>{{ toast.message }}</span>
          <button type="button" class="toast-close" (click)="notifications.dismiss(toast.id)" aria-label="Fermer">×</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-stack {
      position: fixed;
      top: var(--spacing-4);
      right: var(--spacing-4);
      display: flex;
      flex-direction: column;
      gap: var(--spacing-2);
      z-index: 1000;
      max-width: 360px;
    }
    .toast {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: var(--spacing-3);
      padding: var(--spacing-3) var(--spacing-4);
      border-radius: var(--radius-sm);
      color: #fff;
      font-size: var(--font-size-body);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    }
    .toast-error { background: var(--color-danger); }
    .toast-success { background: var(--color-success); }
    .toast-close {
      background: transparent;
      border: none;
      color: #fff;
      font-size: 18px;
      line-height: 1;
      cursor: pointer;
      min-height: auto;
      min-width: auto;
      padding: 0;
    }
  `]
})
export class ToastContainerComponent {
  notifications = inject(NotificationService);
}
