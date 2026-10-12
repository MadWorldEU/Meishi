import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AppSettings } from '../../shared/app-settings';
import { Home } from './home';
import { CV } from '../../cv';

const CURRICULUM_VITAE_URL = 'https://api.example.com/curricula-vitae';

describe('Home', () => {
  let fixture: ComponentFixture<Home>;
  let compiled: HTMLElement;
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Home],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
    TestBed.inject(AppSettings).apiUrl = 'https://api.example.com';

    fixture = TestBed.createComponent(Home);
    http = TestBed.inject(HttpTestingController);
    await fixture.whenStable();
    compiled = fixture.nativeElement as HTMLElement;
  });

  const curriculumVitaeRequest = () => http.expectOne({ method: 'GET', url: CURRICULUM_VITAE_URL });
  const header = () => compiled.querySelector('header');

  const texts = (selector: string): string[] =>
    Array.from(compiled.querySelectorAll(selector), (element) => element.textContent?.trim() ?? '');

  it('should create the page', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should show only the terminal loader while waiting for the API', () => {
    expect(compiled.querySelector('.loader')?.textContent).toContain('fetching curriculum vitae');
    expect(compiled.querySelector('.prompt__command')?.textContent).toBe(
      'curl -s /curricula-vitae',
    );
    expect(compiled.querySelector('nav')).toBeNull();
    expect(compiled.querySelector('section')).toBeNull();
  });

  it('should render the full name from the API', async () => {
    curriculumVitaeRequest().flush({ firstName: 'Jane', lastName: 'Roe' });
    await fixture.whenStable();

    expect(compiled.querySelector('h1')?.textContent).toBe('Jane Roe');
    expect(compiled.querySelector('.loader')).toBeNull();
  });

  it('should show an error when the response has no name', async () => {
    curriculumVitaeRequest().flush('<!doctype html>');
    await fixture.whenStable();

    expect(header()?.textContent).toContain('whoami: unexpected response from the API');
    expect(compiled.querySelector('h1')).toBeNull();
  });

  it('should show the status when the API returns an error', async () => {
    curriculumVitaeRequest().flush('', { status: 502, statusText: 'Bad Gateway' });
    await fixture.whenStable();

    expect(header()?.textContent).toContain('whoami: the API responded with status 502');
  });

  it('should show a network error when the API cannot be reached', async () => {
    curriculumVitaeRequest().error(new ProgressEvent('error'));
    await fixture.whenStable();

    expect(header()?.textContent).toContain('whoami: the API could not be reached');
  });

  it('should still render the sections when the API fails', async () => {
    curriculumVitaeRequest().error(new ProgressEvent('error'));
    await fixture.whenStable();

    expect(compiled.querySelector('.loader')).toBeNull();
    expect(compiled.querySelectorAll('section').length).toBe(5);
  });

  describe('once the API has answered', () => {
    beforeEach(async () => {
      curriculumVitaeRequest().flush({ firstName: 'Jane', lastName: 'Roe' });
      await fixture.whenStable();
    });

    it('should render the role and location', () => {
      expect(header()?.textContent).toContain(CV.role);
      expect(header()?.textContent).toContain(CV.location);
    });

    it('should render a navigation link for every section', () => {
      const links = Array.from(compiled.querySelectorAll<HTMLAnchorElement>('nav a'));
      const sections = ['about', 'experience', 'education', 'skills', 'contact'];

      expect(links.map((link) => link.getAttribute('href'))).toEqual(sections.map((s) => `#${s}`));
      expect(links.map((link) => link.textContent?.trim())).toEqual(sections.map((s) => `./${s}`));
    });

    it('should render a section for every navigation link', () => {
      const links = Array.from(compiled.querySelectorAll<HTMLAnchorElement>('nav a'));

      for (const link of links) {
        expect(compiled.querySelector(link.getAttribute('href')!)).not.toBeNull();
      }
    });

    it('should render the about text', () => {
      expect(compiled.querySelector('#about .indent')?.textContent?.trim()).toBe(CV.about);
    });

    it('should render every experience with its highlights', () => {
      const entries = compiled.querySelectorAll('#experience .entry');
      expect(entries.length).toBe(CV.experience.length);

      CV.experience.forEach((job, index) => {
        const entry = entries[index];
        expect(entry.querySelector('.entry__title')?.textContent?.trim()).toBe(
          `${job.role} @ ${job.company}`,
        );
        expect(entry.querySelector('.accent')?.textContent?.trim()).toBe(job.period);

        const highlights = Array.from(entry.querySelectorAll('.entry__list li'), (li) =>
          li.textContent?.trim(),
        );
        expect(highlights).toEqual(job.highlights);
      });
    });

    it('should render every education entry', () => {
      expect(texts('#education .entry__title')).toEqual(CV.education.map((item) => item.degree));
      expect(texts('#education .muted')).toEqual(CV.education.map((item) => item.school));
    });

    it('should render every skill group with its skills', () => {
      const allSkills = CV.skills.flatMap((group) => group.skills);

      expect(texts('.skills__group')).toEqual(CV.skills.map((group) => `# ${group.name}`));
      expect(texts('.skill dt')).toEqual(allSkills.map((skill) => skill.name));
      expect(texts('.skill .muted')).toEqual(allSkills.map((skill) => `${skill.level}/10`));
    });

    it('should render skill levels as ASCII bars', () => {
      const allSkills = CV.skills.flatMap((group) => group.skills);

      expect(texts('.skill__bar')).toEqual(
        allSkills.map((skill) => `[${'#'.repeat(skill.level)}${'-'.repeat(10 - skill.level)}]`),
      );
    });

    it('should render every contact link', () => {
      const links = Array.from(compiled.querySelectorAll<HTMLAnchorElement>('#contact a'));

      expect(links.map((link) => link.getAttribute('href'))).toEqual(
        CV.contact.map((contact) => contact.href),
      );
      expect(links.map((link) => link.textContent?.trim())).toEqual(
        CV.contact.map((contact) => contact.value),
      );
      expect(texts('#contact dt')).toEqual(CV.contact.map((contact) => `${contact.label}:`));
    });
  });
});

describe('Home.bar', () => {
  // bar() is protected; expose it through a subclass so the edge cases can be tested directly.
  class TestableHome extends Home {
    renderBar(level: number): string {
      return this.bar(level);
    }
  }

  let app: TestableHome;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    app = TestBed.runInInjectionContext(() => new TestableHome());
  });

  it.each([
    [0, '[----------]'],
    [1, '[#---------]'],
    [5, '[#####-----]'],
    [10, '[##########]'],
  ])('should render level %i as %s', (level, expected) => {
    expect(app.renderBar(level)).toBe(expected);
  });

  it('should always render a bar of ten characters between brackets', () => {
    for (let level = 0; level <= 10; level++) {
      expect(app.renderBar(level)).toHaveLength(12);
    }
  });
});
