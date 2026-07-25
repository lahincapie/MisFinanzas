import { Component, inject, signal, OnInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { IncomeService } from '../../services/income.service';
import { Income, IncomeRequest } from '../../models/income.models';

@Component({
  selector: 'app-incomes',
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
  templateUrl: './incomes.html',
  styleUrl: './incomes.css'
})
export class Incomes implements OnInit {
  private incomeService = inject(IncomeService);
  private fb = inject(FormBuilder);

  incomes = signal<Income[]>([]);
  editingId = signal<number | null>(null);
  displayedColumns = ['name', 'periodicity', 'receiptDay', 'type', 'expected', 'validity', 'actions'];

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
    periodicity: [1, Validators.required],
    isVariable: [false],
    expectedAmount: [null as number | null, [Validators.required, Validators.min(1)]],
    expectedReceiptDay: [1, [Validators.required, Validators.min(1), Validators.max(31)]],
    startMonth: [''],
    endMonth: ['']
  });

  ngOnInit(): void {
    this.loadIncomes();
  }

  loadIncomes(): void {
    this.incomeService.getAll().subscribe({
      next: (data) => this.incomes.set(data),
      error: (err) => console.error('Error al cargar ingresos:', err)
    });
  }

  startEdit(inc: Income): void {
    this.editingId.set(inc.id);
    this.form.setValue({
      name: inc.name,
      periodicity: inc.periodicity,
      isVariable: inc.isVariable,
      expectedAmount: inc.expectedAmount,
      expectedReceiptDay: inc.expectedReceiptDay,
      startMonth: inc.startMonth ?? '',
      endMonth: inc.endMonth ?? ''
    });
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.form.reset({ periodicity: 1, isVariable: false, expectedReceiptDay: 1 });
  }

  deleteIncome(inc: Income): void {
    if (!confirm(`¿Eliminar el ingreso "${inc.name}"?`)) return;
    this.incomeService.delete(inc.id).subscribe({
      next: () => this.loadIncomes(),
      error: (err) => console.error('Error al eliminar ingreso:', err)
    });
  }
  onSubmit(): void {
    if (this.form.invalid) return;

    const v = this.form.getRawValue();
    const request: IncomeRequest = {
      name: v.name!,
      periodicity: v.periodicity!,
      isVariable: v.isVariable!,
      expectedAmount: v.expectedAmount,
      expectedReceiptDay: v.expectedReceiptDay!,
      startMonth: v.startMonth || null,
      endMonth: v.endMonth || null
    };

    const id = this.editingId();
    const call = id === null
      ? this.incomeService.create(request)
      : this.incomeService.update(id, request);

    call.subscribe({
      next: () => {
        this.cancelEdit();
        this.loadIncomes();
      },
      error: (err) => console.error('Error al guardar ingreso:', err)
    });
  }
}