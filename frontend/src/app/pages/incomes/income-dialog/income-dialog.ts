import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Income, IncomeRequest } from '../../../models/income.models';
import { MONTHS, YEARS, PERIODICITIES } from '../../../shared/options';
import { anchorRequiredValidator } from '../../../shared/validators';

@Component({
  selector: 'app-income-dialog',
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
  templateUrl: './income-dialog.html',
  styleUrl: './income-dialog.css'
})
export class IncomeDialog {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<IncomeDialog>);
  data = inject<Income | null>(MAT_DIALOG_DATA);

  isMonthlySig = signal(true);

  readonly periodicities = PERIODICITIES;
  readonly months = MONTHS;
  readonly years = YEARS;

  form = this.fb.group({
    name: ['', Validators.required],
    periodicity: [1, Validators.required],
    isVariable: [false],
    expectedAmount: [null as number | null, [Validators.required, Validators.min(1)]],
    expectedReceiptDay: [1, [Validators.required, Validators.min(1), Validators.max(31)]],
    startMonthNum: [''],
    startYear: [''],
    endMonthNum: [''],
    endYear: [''],
    anchorMonthNum: [''],
    anchorYear: ['']
  }, { validators: anchorRequiredValidator });

  isEdit = this.data !== null;

  constructor() {
    this.form.controls.periodicity.valueChanges.subscribe(p => {
      this.isMonthlySig.set(Number(p) === 1);
    });

    if (this.data) {
      const i = this.data;
      this.form.setValue({
        name: i.name,
        periodicity: i.periodicity,
        isVariable: i.isVariable,
        expectedAmount: i.expectedAmount,
        expectedReceiptDay: i.expectedReceiptDay,
        startMonthNum: i.startMonth ? i.startMonth.split('-')[1] : '',
        startYear: i.startMonth ? i.startMonth.split('-')[0] : '',
        endMonthNum: i.endMonth ? i.endMonth.split('-')[1] : '',
        endYear: i.endMonth ? i.endMonth.split('-')[0] : '',
        anchorMonthNum: i.anchorMonth ? i.anchorMonth.split('-')[1] : '',
        anchorYear: i.anchorMonth ? i.anchorMonth.split('-')[0] : ''
      });
      this.isMonthlySig.set(Number(i.periodicity) === 1);
    }
  }

  save(): void {
    if (this.form.invalid) return;
    const v = this.form.getRawValue();
    const startMonth = v.startYear && v.startMonthNum ? `${v.startYear}-${v.startMonthNum}` : null;
    const endMonth = v.endYear && v.endMonthNum ? `${v.endYear}-${v.endMonthNum}` : null;
    const anchorMonth = Number(v.periodicity) !== 1 && v.anchorYear && v.anchorMonthNum
      ? `${v.anchorYear}-${v.anchorMonthNum}` : null;
    const request: IncomeRequest = {
      name: v.name!,
      periodicity: v.periodicity!,
      isVariable: v.isVariable!,
      expectedAmount: v.expectedAmount,
      expectedReceiptDay: v.expectedReceiptDay!,
      startMonth,
      endMonth,
      anchorMonth
    };
    this.dialogRef.close(request);
  }

  cancel(): void {
    this.dialogRef.close();
  }
}