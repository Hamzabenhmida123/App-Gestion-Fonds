import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  template: `<span class="badge" [style.color]="color" [style.background]="tint">{{ label }}</span>`
})
export class StatusBadgeComponent {
  @Input({ required: true }) label!: string;
  @Input() color = 'var(--color-bg-secondary)';

  get tint(): string {
    return `color-mix(in srgb, ${this.color} 14%, white)`;
  }
}
