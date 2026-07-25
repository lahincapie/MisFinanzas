import { Component, inject, signal, OnInit } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { DashboardService } from '../../services/dashboard.service';
import { ExpenseService } from '../../services/expense.service';
import { IncomeService } from '../../services/income.service';
import { PaymentMethodService } from '../../services/payment-method.service';
import { DashboardData, DashboardExpenseItem, DashboardIncomeItem } from '../../models/dashboard.models';
import { ExpensePaymentRequest } from '../../models/expense.models';
import { IncomeReceiptRequest } from '../../models/income.models';
import { PaymentMethod } from '../../models/payment-method.models';

@Component({
  selector: 'app-dashboard',
  imports: [
    CurrencyPipe, DatePipe, ReactiveFormsModule,
    MatCardModule, MatTableModule, MatButtonModule,
    MatFormFieldModule, MatInputModule, MatSelectModule
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardService);
  private expenseService = inject(ExpenseService);
  private incomeService = inject(IncomeService);
  private paymentMethodService = inject(PaymentMethodService);
  private fb = inject(FormBuilder);

  data = signal<DashboardData | null>(null);
  currentMonth = signal('');
  payingExpenseId = signal<number | null>(null);
  receivingIncomeId = signal<number | null>(null);
  paymentMethods = signal<PaymentMethod[]>([]);

  expenseColumns = ['name', 'categoryName', 'status', 'amount', 'flag', 'actions'];
  incomeColumns = ['name', 'status', 'amount', 'flag', 'actions'];

  payForm = this.fb.group({
    amount: [null as number | null, [Validators.required, Validators.min(1)]],
    paymentDate: [this.today(), Validators.required],
    paymentMethodId: [null as number | null, Validators.required],
    notes: ['']
  });

  receiveForm = this.fb.group({
    amount: [null as number | null, [Validators.required, Validators.min(1)]],
    receiptDate: [this.today(), Validators.required],
    notes: ['']
  });

  ngOnInit(): void {
    this.currentMonth.set(this.getCurrentMonth());
    this.loadDashboard();
    this.loadPaymentMethods();
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

  today(): string {
    const now = new Date();
    const y = now.getFullYear();
    const m = String(now.getMonth() + 1).padStart(2, '0');
    const d = String(now.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
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

  startPay(exp: DashboardExpenseItem): void {
    this.payingExpenseId.set(exp.expenseId);
    this.payForm.reset({
      amount: exp.projectedAmount || null,
      paymentDate: this.today(),
      paymentMethodId: null,
      notes: ''
    });
  }

  cancelPay(): void {
    this.payingExpenseId.set(null);
  }

  submitPay(): void {
    if (this.payForm.invalid) return;
    const id = this.payingExpenseId();
    if (id === null) return;

    const v = this.payForm.getRawValue();
    const request: ExpensePaymentRequest = {
      amount: v.amount!,
      paymentDate: v.paymentDate!,
      paymentMethodId: v.paymentMethodId!,
      notes: v.notes || null
    };

    this.expenseService.pay(id, this.currentMonth(), request).subscribe({
      next: () => {
        this.payingExpenseId.set(null);
        this.loadDashboard();
      },
      error: (err) => console.error('Error al pagar:', err)
    });
  }

  revertExpense(exp: DashboardExpenseItem): void {
    if (!confirm(`¿Revertir el pago de "${exp.name}"?`)) return;
    this.expenseService.revert(exp.expenseId, this.currentMonth()).subscribe({
      next: () => this.loadDashboard(),
      error: (err) => console.error('Error al revertir:', err)
    });
  }

  startReceive(inc: DashboardIncomeItem): void {
    this.receivingIncomeId.set(inc.incomeId);
    this.receiveForm.reset({
      amount: inc.projectedAmount || null,
      receiptDate: this.today(),
      notes: ''
    });
  }

  cancelReceive(): void {
    this.receivingIncomeId.set(null);
  }

  submitReceive(): void {
    if (this.receiveForm.invalid) return;
    const id = this.receivingIncomeId();
    if (id === null) return;

    const v = this.receiveForm.getRawValue();
    const request: IncomeReceiptRequest = {
      amount: v.amount!,
      receiptDate: v.receiptDate!,
      notes: v.notes || null
    };

    this.incomeService.receive(id, this.currentMonth(), request).subscribe({
      next: () => {
        this.receivingIncomeId.set(null);
        this.loadDashboard();
      },
      error: (err) => console.error('Error al recibir:', err)
    });
  }

  revertIncome(inc: DashboardIncomeItem): void {
    if (!confirm(`¿Revertir la recepción de "${inc.name}"?`)) return;
    this.incomeService.revert(inc.incomeId, this.currentMonth()).subscribe({
      next: () => this.loadDashboard(),
      error: (err) => console.error('Error al revertir:', err)
    });
  }

  loadDashboard(): void {
    this.dashboardService.get(this.currentMonth()).subscribe({
      next: (d) => this.data.set(d),
      error: (err) => console.error('Error al cargar dashboard:', err)
    });
  }
}