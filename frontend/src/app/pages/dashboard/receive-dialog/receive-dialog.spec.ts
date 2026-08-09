import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceiveDialog } from './receive-dialog';

describe('ReceiveDialog', () => {
  let component: ReceiveDialog;
  let fixture: ComponentFixture<ReceiveDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReceiveDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(ReceiveDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
