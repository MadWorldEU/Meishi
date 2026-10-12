import { Routes } from '@angular/router';
import { Home } from './pages/home/home';

export const routes: Routes = [
  { path: '', title: 'CV', component: Home },
  {
    path: 'debug',
    title: 'Debug',
    loadComponent: () => import('./pages/debug/debug').then((m) => m.Debug),
  },
];
