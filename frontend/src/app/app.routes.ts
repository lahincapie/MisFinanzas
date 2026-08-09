import { Routes } from '@angular/router';
import { AuthLayout } from './auth-layout/auth-layout';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Layout } from './layout/layout';
import { Dashboard } from './pages/dashboard/dashboard';
import { Categories } from './pages/categories/categories';
import { Expenses } from './pages/expenses/expenses';
import { Incomes } from './pages/incomes/incomes';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: '',
    component: AuthLayout,
    children: [
      { path: 'login', component: Login },
      { path: 'register', component: Register }
    ]
  },
  {
    path: '',
    component: Layout,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: Dashboard },
      { path: 'categories', component: Categories },
      { path: 'expenses', component: Expenses },
      { path: 'incomes', component: Incomes }
    ]
  }
];