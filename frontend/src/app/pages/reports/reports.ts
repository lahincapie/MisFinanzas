import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ReportService } from '../../services/report.service';
import { CategorySpend, ExpenseSpend } from '../../models/report.models';
import { MONTHS, YEARS } from '../../shared/options';

interface ReportRow { label: string; sub: string; total: number; }

@Component({
  selector: 'app-reports',
  imports: [
    CurrencyPipe, DecimalPipe, FormsModule,
    MatFormFieldModule, MatSelectModule, MatButtonModule, MatProgressSpinnerModule
  ],
  templateUrl: './reports.html',
  styleUrl: './reports.css'
})
export class Reports implements OnInit {
  private reportService = inject(ReportService);

  readonly months = MONTHS;
  readonly years = YEARS;

  private now = new Date();
  fromMonth = '01';
  fromYear = String(this.now.getFullYear());
  toMonth = String(this.now.getMonth() + 1).padStart(2, '0');
  toYear = String(this.now.getFullYear());

  view = signal<'category' | 'expense'>('category');
  categoryData = signal<CategorySpend[]>([]);
  expenseData = signal<ExpenseSpend[]>([]);
  loading = signal(false);

  rows = computed<ReportRow[]>(() =>
    this.view() === 'category'
      ? this.categoryData().map(r => ({ label: r.categoryName, sub: '', total: r.total }))
      : this.expenseData().map(r => ({ label: r.expenseName, sub: r.categoryName, total: r.total }))
  );

  total = computed(() => this.rows().reduce((s, r) => s + r.total, 0));
  maxTotal = computed(() => this.rows().reduce((m, r) => Math.max(m, r.total), 0));

  ngOnInit(): void { this.load(); }

  setView(v: 'category' | 'expense'): void {
    if (this.view() === v) return;
    this.view.set(v);
    this.load();
  }

  load(): void {
    const from = `${this.fromYear}-${this.fromMonth}`;
    const to = `${this.toYear}-${this.toMonth}`;
    this.loading.set(true);
    if (this.view() === 'category') {
      this.reportService.byCategory(from, to).subscribe({
        next: (d) => { this.categoryData.set(d); this.loading.set(false); },
        error: (err) => { console.error('Error al cargar el reporte:', err); this.loading.set(false); }
      });
    } else {
      this.reportService.byExpense(from, to).subscribe({
        next: (d) => { this.expenseData.set(d); this.loading.set(false); },
        error: (err) => { console.error('Error al cargar el reporte:', err); this.loading.set(false); }
      });
    }
  }

  barWidth(total: number): number {
    const max = this.maxTotal();
    return max > 0 ? (total / max) * 100 : 0;
  }

  percent(total: number): number {
    const t = this.total();
    return t > 0 ? (total / t) * 100 : 0;
  }
}