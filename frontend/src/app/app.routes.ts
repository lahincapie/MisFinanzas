import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Dashboard } from './pages/dashboard/dashboard';
import { Categories } from './pages/categories/categories';
import { Expenses } from './pages/expenses/expenses';
import { Incomes } from './pages/incomes/incomes';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
  { path: 'categories', component: Categories, canActivate: [authGuard] },
  { path: 'expenses', component: Expenses, canActivate: [authGuard] },
  { path: 'incomes', component: Incomes, canActivate: [authGuard] },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];