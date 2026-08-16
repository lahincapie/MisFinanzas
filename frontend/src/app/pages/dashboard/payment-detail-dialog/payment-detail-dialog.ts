import { Component, inject } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface PaymentDetailData {
  kind: 'expense' | 'income';
  title: string;
  subtitle: string | null;
  amount: number | null;
  date: string | null;
  paymentMethodName: string | null;
  notes: string | null;
}

@Component({
  selector: 'app-payment-detail-dialog',
  imports: [CurrencyPipe, DatePipe, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './payment-detail-dialog.html',
  styleUrl: './payment-detail-dialog.css'
})
export class PaymentDetailDialog {
  data = inject<PaymentDetailData>(MAT_DIALOG_DATA);
}