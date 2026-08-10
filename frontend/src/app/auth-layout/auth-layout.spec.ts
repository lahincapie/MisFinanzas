import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AuthLayout } from './auth-layout';

describe('AuthLayout', () => {
  it('should create', async () => {
    await TestBed.configureTestingModule({
      imports: [AuthLayout],
      providers: [provideRouter([])]
    }).compileComponents();

    const fixture = TestBed.createComponent(AuthLayout);
    expect(fixture.componentInstance).toBeTruthy();
  });
});