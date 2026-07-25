import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Income, IncomeRequest, IncomeReceiptRequest } from '../models/income.models';

@Injectable({ providedIn: 'root' })
export class IncomeService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/incomes`;

  getAll(): Observable<Income[]> {
    return this.http.get<Income[]>(this.baseUrl);
  }

  create(data: IncomeRequest): Observable<Income> {
    return this.http.post<Income>(this.baseUrl, data);
  }

  update(id: number, data: IncomeRequest): Observable<Income> {
    return this.http.put<Income>(`${this.baseUrl}/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
  generateMonthly(month: string): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/generate-monthly`, null, { params: { month } });
  }

  receive(id: number, month: string, data: IncomeReceiptRequest): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/${id}/months/${month}/receive`, data);
  }

  revert(id: number, month: string): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/${id}/months/${month}/revert`, null);
  }
}