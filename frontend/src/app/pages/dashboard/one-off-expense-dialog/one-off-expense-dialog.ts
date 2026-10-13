import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { PaymentMethod } from '../../../models/payment-method.models';
import { OneOffExpenseRequest } from '../../../models/expense.models';

interface OneOffExpenseDialogData {
  paymentMethods: PaymentMethod[];
}

@Component({
  selector: 'app-one-off-expense-dialog',
  imports: [
    ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatIconModule, MatButtonModule
  ],
  templateUrl: './one-off-expense-dialog.html',
  styleUrl: './one-off-expense-dialog.css'
})
export class OneOffExpenseDialog {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<OneOffExpenseDialog>);
  data = inject<OneOffExpenseDialogData>(MAT_DIALOG_DATA);

  form = this.fb.group({
    name: ['', Validators.required],
    amount: [null as number | null, [Validators.required, Validators.min(1)]],
    paymentMethodId: [null as number | null, Validators.required],
    notes: ['']
  });

  save(): void {
    if (this.form.invalid) return;
    const v = this.form.getRawValue();
    const request: OneOffExpenseRequest = {
      name: v.name!,
      amount: v.amount!,
      paymentMethodId: v.paymentMethodId!,
      notes: v.notes || null
    };
    this.dialogRef.close(request);
  }

  cancel(): void {
    this.dialogRef.close();
  }
}