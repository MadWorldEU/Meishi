import { Component } from '@angular/core';
import { CV } from '../../cv';

@Component({
  selector: 'app-home',
  styleUrl: './home.scss',
  templateUrl: './home.html',
})
export class Home {
  protected readonly cv = CV;
  protected readonly sections = ['about', 'experience', 'education', 'skills', 'contact'];

  /** Renders a skill level (0-10) as an ASCII bar, e.g. [########--]. */
  protected bar(level: number): string {
    return `[${'#'.repeat(level)}${'-'.repeat(10 - level)}]`;
  }
}
