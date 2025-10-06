import { Routes } from '@angular/router';

export const routes: Routes = [
  { 
    path: '', 
    redirectTo: '/dashboard', 
    pathMatch: 'full' 
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'people',
    loadComponent: () => import('./components/people/people.component').then(m => m.PeopleComponent)
  },
  {
    path: 'astronaut-duties',
    loadComponent: () => import('./components/astronaut-duties/astronaut-duties.component').then(m => m.AstronautDutiesComponent)
  },
  {
    path: 'reports',
    loadComponent: () => import('./components/reports/reports.component').then(m => m.ReportsComponent)
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
