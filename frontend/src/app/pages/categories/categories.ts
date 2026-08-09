import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { CategoryService } from '../../services/category.service';
import { ExpenseService } from '../../services/expense.service';
import { Category, CategoryRequest } from '../../models/category.models';
import { Expense } from '../../models/expense.models';
import { CategoryDialog } from './category-dialog/category-dialog';
import { ConfirmDialog } from '../../confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-categories',
  imports: [MatIconModule, MatButtonModule],
  templateUrl: './categories.html',
  styleUrl: './categories.css'
})
export class Categories implements OnInit {
  private categoryService = inject(CategoryService);
  private expenseService = inject(ExpenseService);
  private dialog = inject(MatDialog);

  categories = signal<Category[]>([]);
  expenses = signal<Expense[]>([]);
  errorMessage = signal('');

  searchTerm = signal('');

  filteredCategories = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const cats = this.categories();
    if (!term) return cats;
    return cats.filter(c => c.name.toLowerCase().includes(term));
  });

  ngOnInit(): void {
    this.loadCategories();
    this.loadExpenses();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => this.categories.set(
        [...data].sort((a, b) => a.name.localeCompare(b.name))
      ),
      error: (err) => console.error('Error al cargar categorías:', err)
    });
  }

  loadExpenses(): void {
    this.expenseService.getAll().subscribe({
      next: (data) => this.expenses.set(data),
      error: (err) => console.error('Error al cargar gastos:', err)
    });
  }

  usageCount(categoryId: number): number {
    return this.expenses().filter(e => e.categoryId === categoryId).length;
  }

  openDialog(cat?: Category): void {
    const ref = this.dialog.open(CategoryDialog, { data: cat ?? null });
    ref.afterClosed().subscribe((result: CategoryRequest | undefined) => {
      if (!result) return;
      const call = cat
        ? this.categoryService.update(cat.id, result)
        : this.categoryService.create(result);
      call.subscribe({
        next: () => this.loadCategories(),
        error: (err) => console.error('Error al guardar:', err)
      });
    });
  }

  deleteCategory(cat: Category): void {
    const ref = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Desactivar categoría',
        message: `¿Desactivar la categoría <strong>${cat.name}</strong>? Dejará de aparecer en los listados (soft-delete). Podrás reactivarla después; nada se borra de verdad.`,
        confirmLabel: 'Desactivar',
        icon: 'delete',
        danger: true
      }
    });
    ref.afterClosed().subscribe(ok => {
      if (!ok) return;
      this.errorMessage.set('');
      this.categoryService.delete(cat.id).subscribe({
        next: () => this.loadCategories(),
        error: (err) => {
          if (err.status === 409) {
            this.errorMessage.set('No se puede eliminar: la categoría está en uso por gastos.');
          } else {
            this.errorMessage.set('No se pudo eliminar la categoría.');
          }
        }
      });
    });
  }
}