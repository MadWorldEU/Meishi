import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AppSettings } from '../../shared/app-settings';
import { Home } from './home';
import { CurriculumVitae } from '../../cv';

const CURRICULUM_VITAE_URL = 'https://api.example.com/curricula-vitae';

const CV: CurriculumVitae = {
  firstName: 'Jane',
  lastName: 'Roe',
  role: 'Staff Engineer',
  location: 'Utrecht',
  about: 'Builds things that last.',
  experience: [
    {
      period: '2022 - present',
      role: 'Staff Engineer',
      company: 'Acme',
      highlights: ['Led the platform team.', 'Migrated everything to .NET.'],
    },
    { period: '2018 - 2022', role: 'Developer', company: 'Globex', highlights: ['Shipped often.'] },
  ],
  education: [{ period: '2014 - 2018', degree: 'MSc Software Engineering', school: 'TU Delft' }],
  skills: [
    {
      name: 'backend',
      skills: [
        { name: 'C#', level: 10 },
        { name: 'PostgreSQL', level: 0 },
      ],
    },
    { name: 'frontend', skills: [{ name: 'Angular', level: 6 }] },
  ],
  contact: [
    { label: 'email', value: 'jane@example.com', href: 'mailto:jane@example.com' },
    { label: 'github', value: 'github.com/jane', href: 'https://github.com/jane' },
  ],
  languages: [
    { name: 'Dutch', level: 'MotherTongue' },
    { name: 'English', level: 'Expert' },
    { name: 'French', level: 'Comfortable' },
    { name: 'German', level: 'Beginner' },
  ],
  hobbies: ['Running', 'Photography'],
};

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
    curriculumVitaeRequest().flush(CV);
    await fixture.whenStable();

    expect(compiled.querySelector('h1')?.textContent).toBe('Jane Roe');
    expect(compiled.querySelector('.loader')).toBeNull();
  });

  it('should show the full name in the browser tab', async () => {
    curriculumVitaeRequest().flush(CV);
    await fixture.whenStable();

    expect(document.title).toBe('Jane Roe · CV');
  });

  it('should not change the browser tab title when the API fails', async () => {
    document.title = 'CV';
    curriculumVitaeRequest().error(new ProgressEvent('error'));
    await fixture.whenStable();

    expect(document.title).toBe('CV');
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

  it('should not render the sections when the API fails', async () => {
    curriculumVitaeRequest().error(new ProgressEvent('error'));
    await fixture.whenStable();

    expect(compiled.querySelector('.loader')).toBeNull();
    expect(compiled.querySelector('nav')).toBeNull();
    expect(compiled.querySelector('section')).toBeNull();
    expect(compiled.querySelector('.cursor')).not.toBeNull();
  });

  describe('once the API has answered', () => {
    beforeEach(async () => {
      curriculumVitaeRequest().flush(CV);
      await fixture.whenStable();
    });

    it('should render the role and location', () => {
      expect(header()?.textContent).toContain(CV.role);
      expect(header()?.textContent).toContain(CV.location);
    });

    it('should render a navigation link for every section', () => {
      const links = Array.from(compiled.querySelectorAll<HTMLAnchorElement>('nav a'));
      const sections = [
        'about',
        'experience',
        'education',
        'skills',
        'languages',
        'hobbies',
        'contact',
      ];

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

    it('should not offer to show more experience when there are only a few', () => {
      expect(compiled.querySelector('#experience .more')).toBeNull();
      expect(compiled.querySelector('#experience .prompt__command')?.textContent).toBe(
        'ls -l ~/experience',
      );
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

    it('should render every language with its level', () => {
      expect(texts('#languages dt')).toEqual(CV.languages.map((language) => language.name));
      expect(texts('#languages dd')).toEqual([
        'mother tongue',
        'expert',
        'comfortable',
        'beginner',
      ]);
    });

    it('should render every hobby', () => {
      expect(texts('#hobbies li')).toEqual(CV.hobbies);
    });

    it('should place hobbies next to languages', () => {
      const columns = Array.from(compiled.querySelectorAll('.columns > section'), (s) => s.id);

      expect(columns).toEqual(['languages', 'hobbies']);
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

describe('Home experience', () => {
  const experience = Array.from({ length: 5 }, (_, index) => ({
    period: `${2020 - index}`,
    role: 'Developer',
    company: `Company ${index + 1}`,
    highlights: [],
  }));

  let fixture: ComponentFixture<Home>;
  let compiled: HTMLElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Home],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
    TestBed.inject(AppSettings).apiUrl = 'https://api.example.com';

    fixture = TestBed.createComponent(Home);
    await fixture.whenStable();
    TestBed.inject(HttpTestingController)
      .expectOne(CURRICULUM_VITAE_URL)
      .flush({ ...CV, experience });
    await fixture.whenStable();
    compiled = fixture.nativeElement as HTMLElement;
  });

  const companies = () =>
    Array.from(compiled.querySelectorAll('#experience .entry__title'), (title) =>
      title.textContent?.trim(),
    );
  const toggle = () => compiled.querySelector<HTMLButtonElement>('#experience .more')!;

  it('should show only the first three experiences by default', () => {
    expect(companies()).toEqual([
      'Developer @ Company 1',
      'Developer @ Company 2',
      'Developer @ Company 3',
    ]);
    expect(compiled.querySelector('#experience .prompt__command')?.textContent).toBe(
      'ls -l ~/experience | head -3',
    );
    expect(toggle().textContent?.trim()).toBe('[+] show 2 more');
    expect(toggle().getAttribute('aria-expanded')).toBe('false');
  });

  it('should show all experiences after asking for more', async () => {
    toggle().click();
    await fixture.whenStable();

    expect(companies()).toEqual(experience.map((job) => `Developer @ ${job.company}`));
    expect(compiled.querySelector('#experience .prompt__command')?.textContent).toBe(
      'ls -l ~/experience',
    );
    expect(toggle().textContent?.trim()).toBe('[-] show less');
    expect(toggle().getAttribute('aria-expanded')).toBe('true');
  });

  it('should collapse back to three experiences', async () => {
    toggle().click();
    await fixture.whenStable();
    toggle().click();
    await fixture.whenStable();

    expect(companies()).toHaveLength(3);
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
