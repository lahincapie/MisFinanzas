import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    sessionStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();     // no debe quedar ninguna petición pendiente
    sessionStorage.clear();
  });

  it('sin sesión, isLoggedIn() es false y no hay token', () => {
    expect(service.isLoggedIn()).toBe(false);
    expect(service.getToken()).toBeNull();
  });

  it('login guarda el token y activa la sesión', () => {
    service.login({ email: 'ale@test.com', password: '123' }).subscribe();

    const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
    expect(req.request.method).toBe('POST');
    req.flush({ userId: '1', email: 'ale@test.com', token: 'token-de-prueba' });

    expect(service.getToken()).toBe('token-de-prueba');
    expect(service.isLoggedIn()).toBe(true);
  });

  it('logout borra el token', () => {
    service.login({ email: 'ale@test.com', password: '123' }).subscribe();
    httpMock.expectOne(`${environment.apiUrl}/auth/login`)
      .flush({ userId: '1', email: 'ale@test.com', token: 'tok' });

    service.logout();

    expect(service.getToken()).toBeNull();
    expect(service.isLoggedIn()).toBe(false);
  });
});