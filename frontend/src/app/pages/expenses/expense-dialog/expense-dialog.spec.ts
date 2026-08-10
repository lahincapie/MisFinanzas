import { TestBed } from '@angular/core/testing';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ExpenseDialog } from './expense-dialog';

describe('ExpenseDialog', () => {
  let component: ExpenseDialog;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExpenseDialog],
      providers: [
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: { categories: [], expense: null } }
      ]
    }).compileComponents();

    const fixture = TestBed.createComponent(ExpenseDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('el formulario es inválido con los días en mal orden (20, 10, 5)', () => {
    component.form.patchValue({ cutoffDay: 20, dueDay: 10, suspensionDay: 5 });
    expect(component.form.hasError('daysOrder')).toBe(true);
  });

  it('acepta el orden correcto (5, 10, 20)', () => {
    component.form.patchValue({ cutoffDay: 5, dueDay: 10, suspensionDay: 20 });
    expect(component.form.hasError('daysOrder')).toBe(false);
  });

  it('al encender "variable", el valor esperado deja de ser obligatorio', () => {
    component.form.patchValue({ isVariable: true });
    expect(component.form.controls.expectedAmount.hasError('required')).toBe(false);
  });
});