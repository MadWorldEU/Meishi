import { Routes } from '@angular/router';
import { Home } from './pages/home/home';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'debug', loadComponent: () => import('./pages/debug/debug').then((m) => m.Debug) },
];
