import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PaymentMethod } from '../models/payment-method.models';

@Injectable({ providedIn: 'root' })
export class PaymentMethodService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/payment-methods`;

  getAll(): Observable<PaymentMethod[]> {
    return this.http.get<PaymentMethod[]>(this.baseUrl);
  }
}