import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';

function extractMessage(err: HttpErrorResponse): string {
  const body = err.error;
  if (body && typeof body === 'object' && typeof body.error === 'string') return body.error;
  if (body && typeof body === 'object' && typeof body.title === 'string') return body.title;
  if (typeof body === 'string' && body.trim().length) return body;
  if (err.status === 0) return 'Impossible de contacter le serveur.';
  return `Erreur ${err.status}`;
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notifications = inject(NotificationService);

  return next(req).pipe(
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse) {
        const message = extractMessage(err);
        if (err.status !== 400) {
          notifications.error(message);
        }
        return throwError(() => new Error(message));
      }
      return throwError(() => err);
    })
  );
};
