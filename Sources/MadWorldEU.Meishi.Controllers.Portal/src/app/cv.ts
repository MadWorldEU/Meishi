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

export interface SkillGroup {
  name: string;
  skills: { name: string; level: number }[];
}

export interface ContactLink {
  label: string;
  value: string;
  href: string;
}

// Placeholder content: replace with your own details.
export const CV = {
  name: 'Your Name',
  role: 'Software Engineer',
  location: 'The Netherlands',
  about:
    'Full-stack developer who enjoys building reliable back-ends in .NET and clean front-ends ' +
    'in Angular. Passionate about clean architecture, automated testing and shipping often.',
  experience: [
    {
      period: '2023 - present',
      role: 'Senior Software Engineer',
      company: 'Company A',
      highlights: [
        'Designed and built ASP.NET Core APIs orchestrated with .NET Aspire.',
        'Set up CI/CD pipelines with GitHub Actions and Docker.',
      ],
    },
    {
      period: '2020 - 2023',
      role: 'Software Engineer',
      company: 'Company B',
      highlights: [
        'Developed Angular applications used by thousands of customers.',
        'Introduced unit and integration testing into the team workflow.',
      ],
    },
  ] satisfies Experience[],
  education: [
    {
      period: '2016 - 2020',
      degree: 'BSc Computer Science',
      school: 'University of Applied Sciences',
    },
  ] satisfies Education[],
  skills: [
    {
      name: 'backend',
      skills: [
        { name: 'C# / .NET', level: 9 },
        { name: 'ASP.NET Core', level: 9 },
        { name: 'SQL', level: 7 },
      ],
    },
    {
      name: 'frontend',
      skills: [
        { name: 'Angular', level: 8 },
        { name: 'TypeScript', level: 8 },
        { name: 'SCSS', level: 7 },
      ],
    },
    {
      name: 'devops',
      skills: [
        { name: 'Docker', level: 7 },
        { name: 'GitHub Actions', level: 7 },
      ],
    },
  ] satisfies SkillGroup[],
  contact: [
    { label: 'email', value: 'you@example.com', href: 'mailto:you@example.com' },
    { label: 'github', value: 'github.com/your-name', href: 'https://github.com/your-name' },
    {
      label: 'linkedin',
      value: 'linkedin.com/in/your-name',
      href: 'https://www.linkedin.com/in/your-name',
    },
  ] satisfies ContactLink[],
};
