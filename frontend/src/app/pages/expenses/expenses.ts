import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { ExpenseService } from '../../services/expense.service';
import { CategoryService } from '../../services/category.service';
import { Expense, ExpenseRequest } from '../../models/expense.models';
import { Category } from '../../models/category.models';
import { ExpenseDialog } from './expense-dialog/expense-dialog';
import { PERIODICITY_LABELS } from '../../shared/options';
import { ConfirmDialog } from '../../confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-expenses',
  imports: [CurrencyPipe, MatTableModule, MatIconModule, MatButtonModule],
  templateUrl: './expenses.html',
  styleUrl: './expenses.css'
})
export class Expenses implements OnInit {
  private expenseService = inject(ExpenseService);
  private categoryService = inject(CategoryService);
  private dialog = inject(MatDialog);

  expenses = signal<Expense[]>([]);
  categories = signal<Category[]>([]);
  displayedColumns = ['name', 'categoryName', 'periodicity', 'days', 'expected', 'validity', 'actions'];

  readonly periodicityLabels = PERIODICITY_LABELS;

  searchTerm = signal('');
  categoryFilter = signal<number | 'all'>('all');

  filteredExpenses = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const catFilter = this.categoryFilter();
    return this.expenses().filter(e => {
      const matchesName = !term || e.name.toLowerCase().includes(term);
      const matchesCat = catFilter === 'all' || e.categoryId === catFilter;
      return matchesName && matchesCat;
    });
  });
  ngOnInit(): void {
    this.loadExpenses();
    this.loadCategories();
  }

  loadExpenses(): void {
    this.expenseService.getAll().subscribe({
      next: (data) => this.expenses.set(data),
      error: (err) => console.error('Error al cargar gastos:', err)
    });
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

  openDialog(exp?: Expense): void {
    const ref = this.dialog.open(ExpenseDialog, {
      data: { categories: this.categories(), expense: exp ?? null }
    });
    ref.afterClosed().subscribe((result: ExpenseRequest | undefined) => {
      if (!result) return;
      const call = exp
        ? this.expenseService.update(exp.id, result)
        : this.expenseService.create(result);
      call.subscribe({
        next: () => this.loadExpenses(),
        error: (err) => console.error('Error al guardar gasto:', err)
      });
    });
  }

  deleteExpense(exp: Expense): void {
    const ref = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Desactivar gasto',
        message: `¿Desactivar el gasto <strong>${exp.name}</strong>? Se ocultará de listas y cálculos, y sus pendientes futuros ya generados también se desactivarán (RF-11).`,
        confirmLabel: 'Desactivar',
        icon: 'delete',
        danger: true
      }
    });
    ref.afterClosed().subscribe(ok => {
      if (!ok) return;
      this.expenseService.delete(exp.id).subscribe({
        next: () => this.loadExpenses(),
        error: (err) => console.error('Error al eliminar gasto:', err)
      });
    });
  }
}