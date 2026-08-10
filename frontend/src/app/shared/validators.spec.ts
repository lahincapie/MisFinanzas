import { FormBuilder } from '@angular/forms';
import { daysOrderValidator } from './validators';

describe('daysOrderValidator', () => {
  const fb = new FormBuilder();

  function makeGroup(cutoff: number, due: number, suspension: number) {
    return fb.group({ cutoffDay: cutoff, dueDay: due, suspensionDay: suspension });
  }

  it('devuelve null cuando corte <= pago <= suspensión', () => {
    expect(daysOrderValidator(makeGroup(5, 10, 20))).toBeNull();
  });

  it('acepta valores iguales (5, 5, 5)', () => {
    expect(daysOrderValidator(makeGroup(5, 5, 5))).toBeNull();
  });

  it('devuelve error cuando el orden está al revés', () => {
    expect(daysOrderValidator(makeGroup(20, 10, 5))).toEqual({ daysOrder: true });
  });

  it('devuelve error si el pago supera la suspensión', () => {
    expect(daysOrderValidator(makeGroup(5, 20, 10))).toEqual({ daysOrder: true });
  });
});