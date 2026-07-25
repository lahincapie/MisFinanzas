import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Incomes } from './incomes';

describe('Incomes', () => {
  let component: Incomes;
  let fixture: ComponentFixture<Incomes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Incomes],
    }).compileComponents();

    fixture = TestBed.createComponent(Incomes);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
