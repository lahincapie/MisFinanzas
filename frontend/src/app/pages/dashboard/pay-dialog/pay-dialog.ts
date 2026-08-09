import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { DashboardExpenseItem } from '../../../models/dashboard.models';
import { ExpensePaymentRequest } from '../../../models/expense.models';
import { PaymentMethod } from '../../../models/payment-method.models';

interface PayDialogData {
  expense: DashboardExpenseItem;
  month: string;
  paymentMethods: PaymentMethod[];
}

@Component({
  selector: 'app-pay-dialog',
  imports: [
    ReactiveFormsModule, CurrencyPipe, DatePipe,
    MatDialogModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatIconModule
  ],
  templateUrl: './pay-dialog.html',
  styleUrl: './pay-dialog.css'
})
export class PayDialog {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<PayDialog>);
  data = inject<PayDialogData>(MAT_DIALOG_DATA);

  form = this.fb.group({
    amount: [this.data.expense.projectedAmount || (null as number | null), [Validators.required, Validators.min(1)]],
    paymentDate: [this.today(), Validators.required],
    paymentMethodId: [null as number | null, Validators.required],
    notes: ['']
  });

  today(): string {
    const n = new Date();
    return `${n.getFullYear()}-${String(n.getMonth() + 1).padStart(2, '0')}-${String(n.getDate()).padStart(2, '0')}`;
  }

  save(): void {
    if (this.form.invalid) return;
    const v = this.form.getRawValue();
    const request: ExpensePaymentRequest = {
      amount: v.amount!,
      paymentDate: v.paymentDate!,
      paymentMethodId: v.paymentMethodId!,
      notes: v.notes || null
    };
    this.dialogRef.close(request);
  }

  cancel(): void { this.dialogRef.close(); }
}