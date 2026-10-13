import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { OneOffIncomeRequest } from '../../../models/income.models';

@Component({
  selector: 'app-one-off-income-dialog',
  imports: [
    ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatIconModule, MatButtonModule
  ],
  templateUrl: './one-off-income-dialog.html',
  styleUrl: './one-off-income-dialog.css'
})
export class OneOffIncomeDialog {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<OneOffIncomeDialog>);

  form = this.fb.group({
    name: ['', Validators.required],
    amount: [null as number | null, [Validators.required, Validators.min(1)]],
    notes: ['']
  });

  save(): void {
    if (this.form.invalid) return;
    const v = this.form.getRawValue();
    const request: OneOffIncomeRequest = {
      name: v.name!,
      amount: v.amount!,
      notes: v.notes || null
    };
    this.dialogRef.close(request);
  }

  cancel(): void {
    this.dialogRef.close();
  }
}