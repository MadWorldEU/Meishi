import { Component, inject, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { AppSettings } from '../../shared/app-settings';

export const PING_PATH = '/debug/ping';
export const EXPECTED_PING_RESPONSE = 'pong';

export type PingState =
  | { status: 'idle' }
  | { status: 'loading' }
  | { status: 'success'; body: string; durationMs: number }
  | { status: 'error'; message: string; body?: string };

@Component({
  selector: 'app-debug',
  imports: [RouterLink],
  styleUrl: './debug.scss',
  templateUrl: './debug.html',
})
export class Debug {
  private readonly http = inject(HttpClient);

  protected readonly pingUrl = inject(AppSettings).apiUrl + PING_PATH;
  protected readonly state = signal<PingState>({ status: 'idle' });

  protected ping(): void {
    const startedAt = performance.now();
    this.state.set({ status: 'loading' });

    this.http.get(this.pingUrl, { responseType: 'text' }).subscribe({
      next: (body) => {
        // Anything else than "pong" (e.g. the SPA fallback page) did not come from the API.
        this.state.set(
          body === EXPECTED_PING_RESPONSE
            ? { status: 'success', body, durationMs: Math.round(performance.now() - startedAt) }
            : { status: 'error', message: 'Unexpected response', body },
        );
      },
      error: (error: HttpErrorResponse) => {
        this.state.set({
          status: 'error',
          message:
            error.status === 0
              ? 'Network error: the API could not be reached'
              : `The API responded with status ${error.status}`,
        });
      },
    });
  }
}
