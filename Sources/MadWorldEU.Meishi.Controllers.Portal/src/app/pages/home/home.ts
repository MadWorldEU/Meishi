import { Component, inject, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Title } from '@angular/platform-browser';
import { CV } from '../../cv';
import { AppSettings } from '../../shared/app-settings';

export const CURRICULUM_VITAE_PATH = '/curricula-vitae';

export interface CurriculumVitaeResponse {
  firstName: string;
  lastName: string;
}

export type NameState =
  | { status: 'loading' }
  | { status: 'success'; name: string }
  | { status: 'error'; message: string };

@Component({
  selector: 'app-home',
  styleUrl: './home.scss',
  templateUrl: './home.html',
})
export class Home {
  private readonly http = inject(HttpClient);
  private readonly title = inject(Title);
  private readonly curriculumVitaeUrl = inject(AppSettings).apiUrl + CURRICULUM_VITAE_PATH;

  // Only the name comes from the API so far; the other sections still use the static CV.
  protected readonly cv = CV;
  protected readonly name = signal<NameState>({ status: 'loading' });
  protected readonly sections = ['about', 'experience', 'education', 'skills', 'contact'];

  constructor() {
    this.http.get<CurriculumVitaeResponse>(this.curriculumVitaeUrl).subscribe({
      next: (curriculumVitae) => {
        // A response without names (e.g. the SPA fallback page) did not come from the API.
        if (!curriculumVitae?.firstName || !curriculumVitae?.lastName) {
          this.name.set({ status: 'error', message: 'unexpected response from the API' });
          return;
        }

        const name = `${curriculumVitae.firstName} ${curriculumVitae.lastName}`;
        this.name.set({ status: 'success', name });
        // The route title is just "CV"; put the name in front of it in the browser tab.
        this.title.setTitle(`${name} · CV`);
      },
      error: (error: HttpErrorResponse) => {
        this.name.set({
          status: 'error',
          message:
            error.status === 0
              ? 'the API could not be reached'
              : `the API responded with status ${error.status}`,
        });
      },
    });
  }

  /** Renders a skill level (0-10) as an ASCII bar, e.g. [########--]. */
  protected bar(level: number): string {
    return `[${'#'.repeat(level)}${'-'.repeat(10 - level)}]`;
  }
}
