import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IncomeDialog } from './income-dialog';

describe('IncomeDialog', () => {
  let component: IncomeDialog;
  let fixture: ComponentFixture<IncomeDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IncomeDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(IncomeDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
