import { AbstractControl, ValidationErrors } from '@angular/forms';

/** Verifica que corte <= pago <= suspensión en el formulario de gasto. */
export function daysOrderValidator(group: AbstractControl): ValidationErrors | null {
  const cutoff = group.get('cutoffDay')?.value;
  const due = group.get('dueDay')?.value;
  const suspension = group.get('suspensionDay')?.value;
  if (cutoff == null || due == null || suspension == null) return null;
  return cutoff <= due && due <= suspension ? null : { daysOrder: true };
}