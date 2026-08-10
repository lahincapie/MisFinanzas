import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { Layout } from './layout';

describe('Layout', () => {
  it('should create', async () => {
    await TestBed.configureTestingModule({
      imports: [Layout],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();

    const fixture = TestBed.createComponent(Layout);
    expect(fixture.componentInstance).toBeTruthy();
  });
});