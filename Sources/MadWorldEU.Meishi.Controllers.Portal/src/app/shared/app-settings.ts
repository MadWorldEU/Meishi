import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

/** Locally the dev-server proxy (proxy.conf.js) forwards /api to the API. */
export const DEFAULT_API_URL = '/api';
export const SETTINGS_URL = 'config.json';

/**
 * Runtime settings, loaded from config.json before the app starts.
 * The deployed Portal generates this file from the API_HTTPS environment variable (see Dockerfile).
 */
@Injectable({ providedIn: 'root' })
export class AppSettings {
  private readonly http = inject(HttpClient);

  /** Base URL of the API, without a trailing slash. */
  apiUrl = DEFAULT_API_URL;

  async load(): Promise<void> {
    try {
      const settings = await firstValueFrom(this.http.get<{ apiUrl?: string }>(SETTINGS_URL));
      this.apiUrl = (settings?.apiUrl || DEFAULT_API_URL).replace(/\/+$/, '');
    } catch {
      this.apiUrl = DEFAULT_API_URL;
    }
  }
}
