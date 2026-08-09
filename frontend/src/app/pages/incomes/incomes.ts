import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { IncomeService } from '../../services/income.service';
import { Income, IncomeRequest } from '../../models/income.models';
import { IncomeDialog } from './income-dialog/income-dialog';
import { CurrencyPipe } from '@angular/common';
import { PERIODICITY_LABELS } from '../../shared/options';
import { ConfirmDialog } from '../../confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-incomes',
  imports: [CurrencyPipe, MatTableModule, MatIconModule, MatButtonModule],
  templateUrl: './incomes.html',
  styleUrl: './incomes.css'
})
export class Incomes implements OnInit {
  private incomeService = inject(IncomeService);
  private dialog = inject(MatDialog);

  incomes = signal<Income[]>([]);
  displayedColumns = ['name', 'periodicity', 'receiptDay', 'expected', 'validity', 'actions'];

 readonly periodicityLabels = PERIODICITY_LABELS;

  searchTerm = signal('');

  filteredIncomes = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const items = this.incomes();
    if (!term) return items;
    return items.filter(i => i.name.toLowerCase().includes(term));
  });

  ngOnInit(): void {
    this.loadIncomes();
  }

  loadIncomes(): void {
    this.incomeService.getAll().subscribe({
      next: (data) => this.incomes.set(data),
      error: (err) => console.error('Error al cargar ingresos:', err)
    });
  }

  openDialog(inc?: Income): void {
    const ref = this.dialog.open(IncomeDialog, { data: inc ?? null });
    ref.afterClosed().subscribe((result: IncomeRequest | undefined) => {
      if (!result) return;
      const call = inc
        ? this.incomeService.update(inc.id, result)
        : this.incomeService.create(result);
      call.subscribe({
        next: () => this.loadIncomes(),
        error: (err) => console.error('Error al guardar ingreso:', err)
      });
    });
  }

  deleteIncome(inc: Income): void {
    const ref = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Desactivar ingreso',
        message: `¿Desactivar el ingreso <strong>${inc.name}</strong>? Se ocultará de listas y cálculos, junto con sus pendientes futuros (RF-18).`,
        confirmLabel: 'Desactivar',
        icon: 'delete',
        danger: true
      }
    });
    ref.afterClosed().subscribe(ok => {
      if (!ok) return;
      this.incomeService.delete(inc.id).subscribe({
        next: () => this.loadIncomes(),
        error: (err) => console.error('Error al eliminar ingreso:', err)
      });
    });
  }
}