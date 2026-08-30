import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: '',
    loadComponent: () => import('./auth-layout/auth-layout').then(m => m.AuthLayout),
    children: [
      { path: 'login', loadComponent: () => import('./pages/login/login').then(m => m.Login) },
      { path: 'register', loadComponent: () => import('./pages/register/register').then(m => m.Register) }
    ]
  },
  {
    path: '',
    loadComponent: () => import('./layout/layout').then(m => m.Layout),
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard) },
      { path: 'categories', loadComponent: () => import('./pages/categories/categories').then(m => m.Categories) },
      { path: 'expenses', loadComponent: () => import('./pages/expenses/expenses').then(m => m.Expenses) },
      { path: 'incomes', loadComponent: () => import('./pages/incomes/incomes').then(m => m.Incomes) },
      { path: 'reports', loadComponent: () => import('./pages/reports/reports').then(m => m.Reports) }
    ]
  }
];