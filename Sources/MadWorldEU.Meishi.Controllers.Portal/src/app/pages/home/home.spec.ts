import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Home } from './home';
import { CV } from '../../cv';

describe('Home', () => {
  let fixture: ComponentFixture<Home>;
  let compiled: HTMLElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Home],
    }).compileComponents();

    fixture = TestBed.createComponent(Home);
    await fixture.whenStable();
    compiled = fixture.nativeElement as HTMLElement;
  });

  const texts = (selector: string): string[] =>
    Array.from(compiled.querySelectorAll(selector), (element) => element.textContent?.trim() ?? '');

  it('should create the page', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render the name', () => {
    expect(compiled.querySelector('h1')?.textContent).toContain(CV.name);
  });

  it('should render the role and location', () => {
    const header = compiled.querySelector('header')?.textContent;
    expect(header).toContain(CV.role);
    expect(header).toContain(CV.location);
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

describe('Home.bar', () => {
  // bar() is protected; expose it through a subclass so the edge cases can be tested directly.
  class TestableHome extends Home {
    renderBar(level: number): string {
      return this.bar(level);
    }
  }

  const app = new TestableHome();

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
