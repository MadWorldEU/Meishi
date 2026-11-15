import { Component, inject, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Title } from '@angular/platform-browser';
import { CurriculumVitae } from '../../cv';
import { AppSettings } from '../../shared/app-settings';

export const CURRICULUM_VITAE_PATH = '/curricula-vitae';

/** How many experiences are shown before the visitor asks for the rest. */
export const EXPERIENCE_PREVIEW_COUNT = 3;

export type CurriculumVitaeState =
  | { status: 'loading' }
  | { status: 'success'; name: string; curriculumVitae: CurriculumVitae }
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

  protected readonly state = signal<CurriculumVitaeState>({ status: 'loading' });
  protected readonly sections = ['about', 'experience', 'education', 'skills', 'contact'];
  protected readonly experiencePreviewCount = EXPERIENCE_PREVIEW_COUNT;
  protected readonly showAllExperience = signal(false);

  constructor() {
    this.http.get<CurriculumVitae>(this.curriculumVitaeUrl).subscribe({
      next: (curriculumVitae) => {
        // A response without names (e.g. the SPA fallback page) did not come from the API.
        if (!curriculumVitae?.firstName || !curriculumVitae?.lastName) {
          this.state.set({ status: 'error', message: 'unexpected response from the API' });
          return;
        }

        const name = `${curriculumVitae.firstName} ${curriculumVitae.lastName}`;
        this.state.set({ status: 'success', name, curriculumVitae });
        // The route title is just "CV"; put the name in front of it in the browser tab.
        this.title.setTitle(`${name} · CV`);
      },
      error: (error: HttpErrorResponse) => {
        this.state.set({
          status: 'error',
          message:
            error.status === 0
              ? 'the API could not be reached'
              : `the API responded with status ${error.status}`,
        });
      },
    });
  }

  protected toggleExperience(): void {
    this.showAllExperience.update((showAll) => !showAll);
  }

  /** Renders a skill level (0-10) as an ASCII bar, e.g. [########--]. */
  protected bar(level: number): string {
    return `[${'#'.repeat(level)}${'-'.repeat(10 - level)}]`;
  }
}
