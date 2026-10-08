import { CV } from './cv';

describe('CV', () => {
  it('should have the header fields filled in', () => {
    expect(CV.name.trim()).not.toBe('');
    expect(CV.role.trim()).not.toBe('');
    expect(CV.location.trim()).not.toBe('');
    expect(CV.about.trim()).not.toBe('');
  });

  it('should have skill levels between 0 and 10', () => {
    for (const skill of CV.skills.flatMap((group) => group.skills)) {
      expect(Number.isInteger(skill.level), skill.name).toBe(true);
      expect(skill.level, skill.name).toBeGreaterThanOrEqual(0);
      expect(skill.level, skill.name).toBeLessThanOrEqual(10);
    }
  });

  // The template tracks items by these keys, so duplicates would break rendering.
  it('should have unique tracking keys', () => {
    const unique = (values: string[]) => new Set(values).size === values.length;

    expect(unique(CV.experience.map((job) => job.company + job.period))).toBe(true);
    expect(unique(CV.education.map((item) => item.degree))).toBe(true);
    expect(unique(CV.skills.map((group) => group.name))).toBe(true);
    expect(unique(CV.contact.map((link) => link.label))).toBe(true);
    for (const group of CV.skills) {
      expect(unique(group.skills.map((skill) => skill.name)), group.name).toBe(true);
    }
  });

  it('should only have contact links with a mailto or https scheme', () => {
    for (const link of CV.contact) {
      expect(link.href, link.label).toMatch(/^(mailto:|https:\/\/)/);
    }
  });
});
