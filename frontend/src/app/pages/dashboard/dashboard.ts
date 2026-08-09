import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { forkJoin } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { DashboardService } from '../../services/dashboard.service';
import { ExpenseService } from '../../services/expense.service';
import { IncomeService } from '../../services/income.service';
import { CategoryService } from '../../services/category.service';
import { PaymentMethodService } from '../../services/payment-method.service';
import { Category } from '../../models/category.models';
import { DashboardData, DashboardExpenseItem, DashboardIncomeItem } from '../../models/dashboard.models';
import { ExpensePaymentRequest } from '../../models/expense.models';
import { IncomeReceiptRequest } from '../../models/income.models';
import { PaymentMethod } from '../../models/payment-method.models';
import { PayDialog } from './pay-dialog/pay-dialog';
import { ReceiveDialog } from './receive-dialog/receive-dialog';
import { ConfirmDialog } from '../../confirm-dialog/confirm-dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-dashboard',
  imports: [CurrencyPipe, DatePipe, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardService);
  private expenseService = inject(ExpenseService);
  private incomeService = inject(IncomeService);
  private paymentMethodService = inject(PaymentMethodService);
  private categoryService = inject(CategoryService);
  private dialog = inject(MatDialog);

  data = signal<DashboardData | null>(null);
  currentMonth = signal('');
  paymentMethods = signal<PaymentMethod[]>([]);
  categories = signal<Category[]>([]);

  expensesTotal = computed(() => this.data()?.expenses.length ?? 0);
  expensesPaidCount = computed(() => this.data()?.expenses.filter(e => e.status === 'Pagado').length ?? 0);
  expensesPendingCount = computed(() => this.data()?.expenses.filter(e => e.status === 'Pendiente').length ?? 0);
  incomesTotal = computed(() => this.data()?.incomes.length ?? 0);
  incomesReceivedCount = computed(() => this.data()?.incomes.filter(i => i.status === 'Recibido').length ?? 0);

  expenseFilter = signal<'todos' | 'pendientes' | 'pagados'>('todos');
  incomeFilter = signal<'todos' | 'pendientes' | 'recibidos'>('todos');

  filteredExpenses = computed(() => {
    const items = this.data()?.expenses ?? [];
    const f = this.expenseFilter();
    if (f === 'pendientes') return items.filter(e => e.status === 'Pendiente');
    if (f === 'pagados') return items.filter(e => e.status === 'Pagado');
    return items;
  });

  filteredIncomes = computed(() => {
    const items = this.data()?.incomes ?? [];
    const f = this.incomeFilter();
    if (f === 'pendientes') return items.filter(i => i.status === 'Pendiente');
    if (f === 'recibidos') return items.filter(i => i.status === 'Recibido');
    return items;
  });

  ngOnInit(): void {
    this.currentMonth.set(this.getCurrentMonth());
    this.loadDashboard();
    this.loadPaymentMethods();
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => this.categories.set(data),
      error: (err) => console.error('Error al cargar categorías:', err)
    });
  }

  getCategoryColor(name: string): string {
    return this.categories().find(c => c.name === name)?.color ?? '#8a8a9a';
  }

  getCategoryIcon(name: string): string {
    return this.categories().find(c => c.name === name)?.icon ?? 'category';
  }

  loadPaymentMethods(): void {
    this.paymentMethodService.getAll().subscribe({
      next: (data) => this.paymentMethods.set(data),
      error: (err) => console.error('Error al cargar medios de pago:', err)
    });
  }

  getCurrentMonth(): string {
    const now = new Date();
    return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`;
  }

  changeMonth(delta: number): void {
    const [year, month] = this.currentMonth().split('-').map(Number);
    const date = new Date(year, month - 1 + delta, 1);
    this.currentMonth.set(`${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`);
    this.loadDashboard();
  }

  generatePendings(): void {
    const month = this.currentMonth();
    forkJoin([
      this.expenseService.generateMonthly(month),
      this.incomeService.generateMonthly(month)
    ]).subscribe({
      next: () => this.loadDashboard(),
      error: (err) => console.error('Error al generar pendientes:', err)
    });
  }

  openPayDialog(exp: DashboardExpenseItem): void {
    const ref = this.dialog.open(PayDialog, {
      data: { expense: exp, month: this.currentMonth(), paymentMethods: this.paymentMethods() }
    });
    ref.afterClosed().subscribe((result: ExpensePaymentRequest | undefined) => {
      if (!result) return;
      this.expenseService.pay(exp.expenseId, this.currentMonth(), result).subscribe({
        next: () => this.loadDashboard(),
        error: (err) => console.error('Error al pagar:', err)
      });
    });
  }

  revertExpense(exp: DashboardExpenseItem): void {
    const ref = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Revertir pago',
        message: `¿Revertir el pago de <strong>${exp.name}</strong>? Volverá a quedar pendiente.`,
        confirmLabel: 'Revertir',
        icon: 'undo',
        danger: true
      }
    });
    ref.afterClosed().subscribe(ok => {
      if (!ok) return;
      this.expenseService.revert(exp.expenseId, this.currentMonth()).subscribe({
        next: () => this.loadDashboard(),
        error: (err) => console.error('Error al revertir:', err)
      });
    });
  }

  openReceiveDialog(inc: DashboardIncomeItem): void {
    const ref = this.dialog.open(ReceiveDialog, {
      data: { income: inc, month: this.currentMonth() }
    });
    ref.afterClosed().subscribe((result: IncomeReceiptRequest | undefined) => {
      if (!result) return;
      this.incomeService.receive(inc.incomeId, this.currentMonth(), result).subscribe({
        next: () => this.loadDashboard(),
        error: (err) => console.error('Error al recibir:', err)
      });
    });
  }

  revertIncome(inc: DashboardIncomeItem): void {
    const ref = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Revertir recepción',
        message: `¿Revertir la recepción de <strong>${inc.name}</strong>? Volverá a quedar pendiente.`,
        confirmLabel: 'Revertir',
        icon: 'undo',
        danger: true
      }
    });
    ref.afterClosed().subscribe(ok => {
      if (!ok) return;
      this.incomeService.revert(inc.incomeId, this.currentMonth()).subscribe({
        next: () => this.loadDashboard(),
        error: (err) => console.error('Error al revertir:', err)
      });
    });
  }

  loadDashboard(): void {
    this.dashboardService.get(this.currentMonth()).subscribe({
      next: (d) => this.data.set(d),
      error: (err) => console.error('Error al cargar dashboard:', err)
    });
  }
}