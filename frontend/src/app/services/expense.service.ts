import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Expense, ExpenseRequest, ExpensePaymentRequest } from '../models/expense.models';

@Injectable({ providedIn: 'root' })
export class ExpenseService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/expenses`;

  getAll(): Observable<Expense[]> {
    return this.http.get<Expense[]>(this.baseUrl);
  }

  create(data: ExpenseRequest): Observable<Expense> {
    return this.http.post<Expense>(this.baseUrl, data);
  }

  update(id: number, data: ExpenseRequest): Observable<Expense> {
    return this.http.put<Expense>(`${this.baseUrl}/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
  generateMonthly(month: string): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/generate-monthly`, null, { params: { month } });
  }

  pay(id: number, month: string, data: ExpensePaymentRequest): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/${id}/months/${month}/pay`, data);
  }

  revert(id: number, month: string): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/${id}/months/${month}/revert`, null);
  }
}