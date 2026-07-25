import { Component, inject, signal, OnInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { ExpenseService } from '../../services/expense.service';
import { CategoryService } from '../../services/category.service';
import { Expense, ExpenseRequest } from '../../models/expense.models';
import { Category } from '../../models/category.models';

function daysOrderValidator(group: AbstractControl): ValidationErrors | null {
  const cutoff = group.get('cutoffDay')?.value;
  const due = group.get('dueDay')?.value;
  const suspension = group.get('suspensionDay')?.value;
  if (cutoff == null || due == null || suspension == null) {
    return null;
  }
  return cutoff <= due && due <= suspension ? null : { daysOrder: true };
}

@Component({
  selector: 'app-expenses',
  imports: [
    CurrencyPipe,
    ReactiveFormsModule,
    MatTableModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatButtonModule
  ],
  templateUrl: './expenses.html',
  styleUrl: './expenses.css'
})
export class Expenses implements OnInit {
  private expenseService = inject(ExpenseService);
  private categoryService = inject(CategoryService);
  private fb = inject(FormBuilder);

  expenses = signal<Expense[]>([]);
  categories = signal<Category[]>([]);
  isVariableSig = signal(false);
  editingId = signal<number | null>(null);
  displayedColumns = ['name', 'categoryName', 'periodicity', 'days', 'expected', 'validity', 'actions'];

  readonly periodicityLabels: Record<number, string> = {
    1: 'Mensual', 2: 'Bimestral', 3: 'Trimestral', 4: 'Semestral', 5: 'Anual'
  };

  readonly periodicities = [
    { value: 1, label: 'Mensual' },
    { value: 2, label: 'Bimestral' },
    { value: 3, label: 'Trimestral' },
    { value: 4, label: 'Semestral' },
    { value: 5, label: 'Anual' }
  ];

  form = this.fb.group({
    name: ['', Validators.required],
    categoryId: [null as number | null, Validators.required],
    periodicity: [1, Validators.required],
    isVariable: [false],
    expectedAmount: [null as number | null, [Validators.required, Validators.min(0)]],
    cutoffDay: [1, [Validators.required, Validators.min(1), Validators.max(31)]],
    dueDay: [5, [Validators.required, Validators.min(1), Validators.max(31)]],
    suspensionDay: [10, [Validators.required, Validators.min(1), Validators.max(31)]],
    startMonth: [''],
    endMonth: [''],
    reference: [''],
    contract: ['']
  }, { validators: daysOrderValidator });

  constructor() {
    this.form.controls.isVariable.valueChanges.subscribe(isVar => {
      this.isVariableSig.set(!!isVar);
      const amount = this.form.controls.expectedAmount;
      if (isVar) {
        amount.clearValidators();
        amount.setValue(null);
      } else {
        amount.setValidators([Validators.required, Validators.min(0)]);
      }
      amount.updateValueAndValidity();
    });
  }

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

  startEdit(exp: Expense): void {
    this.editingId.set(exp.id);
    this.form.setValue({
      name: exp.name,
      categoryId: exp.categoryId,
      periodicity: exp.periodicity,
      isVariable: exp.isVariable,
      expectedAmount: exp.expectedAmount,
      cutoffDay: exp.cutoffDay,
      dueDay: exp.dueDay,
      suspensionDay: exp.suspensionDay,
      startMonth: exp.startMonth ?? '',
      endMonth: exp.endMonth ?? '',
      reference: exp.reference ?? '',
      contract: exp.contract ?? ''
    });
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.form.reset({ periodicity: 1, isVariable: false, cutoffDay: 1, dueDay: 5, suspensionDay: 10 });
  }

  deleteExpense(exp: Expense): void {
    if (!confirm(`¿Eliminar el gasto "${exp.name}"?`)) return;
    this.expenseService.delete(exp.id).subscribe({
      next: () => this.loadExpenses(),
      error: (err) => console.error('Error al eliminar gasto:', err)
    });
  }
  onSubmit(): void {
    if (this.form.invalid) return;

    const v = this.form.getRawValue();
    const request: ExpenseRequest = {
      name: v.name!,
      categoryId: v.categoryId!,
      periodicity: v.periodicity!,
      isVariable: v.isVariable!,
      expectedAmount: v.isVariable ? null : v.expectedAmount,
      cutoffDay: v.cutoffDay!,
      dueDay: v.dueDay!,
      suspensionDay: v.suspensionDay!,
      startMonth: v.startMonth || null,
      endMonth: v.endMonth || null,
      reference: v.reference || null,
      contract: v.contract || null
    };

    const id = this.editingId();
    const call = id === null
      ? this.expenseService.create(request)
      : this.expenseService.update(id, request);

    call.subscribe({
      next: () => {
        this.cancelEdit();
        this.loadExpenses();
      },
      error: (err) => console.error('Error al guardar gasto:', err)
    });
  }
}