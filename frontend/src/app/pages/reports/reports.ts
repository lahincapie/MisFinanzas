import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CurrencyPipe, DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { ReportService } from '../../services/report.service';
import { CategoryService } from '../../services/category.service';
import { CategorySpend, ExpenseSpend } from '../../models/report.models';
import { Category } from '../../models/category.models';
import { MONTHS, YEARS } from '../../shared/options';

interface ReportRow { label: string; sub: string; categoryName: string; total: number; }

@Component({
  selector: 'app-reports',
  imports: [
    CurrencyPipe, DecimalPipe, DatePipe, FormsModule,
    MatFormFieldModule, MatSelectModule, MatButtonModule, MatProgressSpinnerModule, MatIconModule
  ],
  templateUrl: './reports.html',
  styleUrl: './reports.css'
})
export class Reports implements OnInit {
  private reportService = inject(ReportService);
  private categoryService = inject(CategoryService);

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
  categories = signal<Category[]>([]);
  loading = signal(false);
  appliedFrom = signal('');
  appliedTo = signal('');

  rows = computed<ReportRow[]>(() =>
    this.view() === 'category'
      ? this.categoryData().map(r => ({ label: r.categoryName, sub: '', categoryName: r.categoryName, total: r.total }))
      : this.expenseData().map(r => ({ label: r.expenseName, sub: r.categoryName, categoryName: r.categoryName, total: r.total }))
  );

  total = computed(() => this.rows().reduce((s, r) => s + r.total, 0));
  maxTotal = computed(() => this.rows().reduce((m, r) => Math.max(m, r.total), 0));

  ngOnInit(): void {
    this.loadCategories();
    this.load();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => this.categories.set(data),
      error: (err) => console.error('Error al cargar categorías:', err)
    });
  }

  colorFor(categoryName: string): string {
    return this.categories().find(c => c.name === categoryName)?.color ?? 'var(--color-primary)';
  }

  iconFor(categoryName: string): string {
    return this.categories().find(c => c.name === categoryName)?.icon ?? 'category';
  }

  setView(v: 'category' | 'expense'): void {
    if (this.view() === v) return;
    this.view.set(v);
    this.load();
  }

  load(): void {
    const from = `${this.fromYear}-${this.fromMonth}`;
    const to = `${this.toYear}-${this.toMonth}`;
    this.appliedFrom.set(from);
    this.appliedTo.set(to);
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