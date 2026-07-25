import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { DashboardData } from '../models/dashboard.models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/dashboard`;

  get(month: string): Observable<DashboardData> {
    return this.http.get<DashboardData>(this.baseUrl, { params: { month } });
  }
}