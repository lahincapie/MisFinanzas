import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayDialog } from './pay-dialog';

describe('PayDialog', () => {
  let component: PayDialog;
  let fixture: ComponentFixture<PayDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PayDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(PayDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
