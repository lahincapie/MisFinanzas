import { AbstractControl, ValidationErrors } from '@angular/forms';

/** Verifica que corte <= pago <= suspensión en el formulario de gasto. */
export function daysOrderValidator(group: AbstractControl): ValidationErrors | null {
  const cutoff = group.get('cutoffDay')?.value;
  const due = group.get('dueDay')?.value;
  const suspension = group.get('suspensionDay')?.value;
  if (cutoff == null || due == null || suspension == null) return null;
  return cutoff <= due && due <= suspension ? null : { daysOrder: true };
}

export function anchorRequiredValidator(group: AbstractControl): ValidationErrors | null {
  const periodicity = Number(group.get('periodicity')?.value);
  // Mensual (1) no necesita ancla
  if (periodicity === 1) return null;
  const monthNum = group.get('anchorMonthNum')?.value;
  const year = group.get('anchorYear')?.value;
  return monthNum && year ? null : { anchorRequired: true };
}