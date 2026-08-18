import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CategorySpend, ExpenseSpend } from '../models/report.models';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/reports`;

  byCategory(from: string, to: string): Observable<CategorySpend[]> {
    return this.http.get<CategorySpend[]>(`${this.baseUrl}/by-category`, { params: { from, to } });
  }

  byExpense(from: string, to: string): Observable<ExpenseSpend[]> {
    return this.http.get<ExpenseSpend[]>(`${this.baseUrl}/by-expense`, { params: { from, to } });
  }
}