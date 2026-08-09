import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Expense, ExpenseRequest } from '../../../models/expense.models';
import { Category } from '../../../models/category.models';
import { MONTHS, YEARS, PERIODICITIES } from '../../../shared/options';

function daysOrderValidator(group: AbstractControl): ValidationErrors | null {
  const cutoff = group.get('cutoffDay')?.value;
  const due = group.get('dueDay')?.value;
  const suspension = group.get('suspensionDay')?.value;
  if (cutoff == null || due == null || suspension == null) return null;
  return cutoff <= due && due <= suspension ? null : { daysOrder: true };
}

interface ExpenseDialogData {
  categories: Category[];
  expense: Expense | null;
}

@Component({
  selector: 'app-expense-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './expense-dialog.html',
  styleUrl: './expense-dialog.css'
})
export class ExpenseDialog {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ExpenseDialog>);
  data = inject<ExpenseDialogData>(MAT_DIALOG_DATA);

  isVariableSig = signal(false);

  readonly periodicities = PERIODICITIES;
  readonly months = MONTHS;
  readonly years = YEARS;

  form = this.fb.group({
    name: ['', Validators.required],
    categoryId: [null as number | null, Validators.required],
    periodicity: [1, Validators.required],
    isVariable: [false],
    expectedAmount: [null as number | null, [Validators.required, Validators.min(0)]],
    cutoffDay: [1, [Validators.required, Validators.min(1), Validators.max(31)]],
    dueDay: [5, [Validators.required, Validators.min(1), Validators.max(31)]],
    suspensionDay: [10, [Validators.required, Validators.min(1), Validators.max(31)]],
    startMonthNum: [''],
    startYear: [''],
    endMonthNum: [''],
    endYear: [''],
    reference: [''],
    contract: ['']
  }, { validators: daysOrderValidator });

  isEdit = this.data.expense !== null;

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

    if (this.data.expense) {
      const e = this.data.expense;
      this.form.setValue({
        name: e.name,
        categoryId: e.categoryId,
        periodicity: e.periodicity,
        isVariable: e.isVariable,
        expectedAmount: e.expectedAmount,
        cutoffDay: e.cutoffDay,
        dueDay: e.dueDay,
        suspensionDay: e.suspensionDay,
        startMonthNum: e.startMonth ? e.startMonth.split('-')[1] : '',
        startYear: e.startMonth ? e.startMonth.split('-')[0] : '',
        endMonthNum: e.endMonth ? e.endMonth.split('-')[1] : '',
        endYear: e.endMonth ? e.endMonth.split('-')[0] : '',
        reference: e.reference ?? '',
        contract: e.contract ?? ''
      });
    }
  }

  save(): void {
    if (this.form.invalid) return;
    const v = this.form.getRawValue();
    const startMonth = v.startYear && v.startMonthNum ? `${v.startYear}-${v.startMonthNum}` : null;
    const endMonth = v.endYear && v.endMonthNum ? `${v.endYear}-${v.endMonthNum}` : null;
    const request: ExpenseRequest = {
      name: v.name!,
      categoryId: v.categoryId!,
      periodicity: v.periodicity!,
      isVariable: v.isVariable!,
      expectedAmount: v.isVariable ? null : v.expectedAmount,
      cutoffDay: v.cutoffDay!,
      dueDay: v.dueDay!,
      suspensionDay: v.suspensionDay!,
      startMonth,
      endMonth,
      reference: v.reference || null,
      contract: v.contract || null
    };
    this.dialogRef.close(request);
  }

  cancel(): void {
    this.dialogRef.close();
  }
}