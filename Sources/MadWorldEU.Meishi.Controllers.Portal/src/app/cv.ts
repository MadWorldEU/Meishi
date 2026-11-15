// Mirrors GetDefaultCurriculumVitaeResponse in MadWorldEU.Meishi.Core.Contracts.

export interface Experience {
  period: string;
  role: string;
  company: string;
  highlights: string[];
}

export interface Education {
  period: string;
  degree: string;
  school: string;
}

export interface Skill {
  name: string;
  /** Proficiency from 0 to 10. */
  level: number;
}

export interface SkillGroup {
  name: string;
  skills: Skill[];
}

export interface ContactLink {
  label: string;
  value: string;
  href: string;
}

export type LanguageLevel = 'MotherTongue' | 'Expert' | 'Comfortable' | 'Beginner';

export interface Language {
  name: string;
  level: LanguageLevel;
}

export interface CurriculumVitae {
  firstName: string;
  lastName: string;
  role: string;
  location: string;
  about: string;
  experience: Experience[];
  education: Education[];
  skills: SkillGroup[];
  contact: ContactLink[];
  languages: Language[];
}
