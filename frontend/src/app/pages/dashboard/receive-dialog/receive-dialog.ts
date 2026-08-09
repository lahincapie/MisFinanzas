import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { DashboardIncomeItem } from '../../../models/dashboard.models';
import { IncomeReceiptRequest } from '../../../models/income.models';

interface ReceiveDialogData {
  income: DashboardIncomeItem;
  month: string;
}

@Component({
  selector: 'app-receive-dialog',
  imports: [
    ReactiveFormsModule, CurrencyPipe, DatePipe,
    MatDialogModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule
  ],
  templateUrl: './receive-dialog.html',
  styleUrl: './receive-dialog.css'
})
export class ReceiveDialog {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ReceiveDialog>);
  data = inject<ReceiveDialogData>(MAT_DIALOG_DATA);

  form = this.fb.group({
    amount: [this.data.income.projectedAmount || (null as number | null), [Validators.required, Validators.min(1)]],
    receiptDate: [this.today(), Validators.required],
    notes: ['']
  });

  today(): string {
    const n = new Date();
    return `${n.getFullYear()}-${String(n.getMonth() + 1).padStart(2, '0')}-${String(n.getDate()).padStart(2, '0')}`;
  }

  save(): void {
    if (this.form.invalid) return;
    const v = this.form.getRawValue();
    const request: IncomeReceiptRequest = {
      amount: v.amount!,
      receiptDate: v.receiptDate!,
      notes: v.notes || null
    };
    this.dialogRef.close(request);
  }

  cancel(): void { this.dialogRef.close(); }
}