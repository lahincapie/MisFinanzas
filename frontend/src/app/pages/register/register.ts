import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../services/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { MessageDialog } from '../../message-dialog/message-dialog';


function passwordsMatch(group: AbstractControl): ValidationErrors | null {
  const pass = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return pass === confirm ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private dialog = inject(MatDialog);

  errorMessage = signal('');
  hidePassword = signal(true);
  hideConfirm = signal(true);

  form = this.fb.nonNullable.group({
    displayName: [''],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: passwordsMatch });

  passwordStrength(): number {
    const p = this.form.controls.password.value;
    let score = 0;
    if (p.length >= 8) score++;
    if (/[A-Z]/.test(p)) score++;
    if (/[0-9]/.test(p)) score++;
    if (/[^A-Za-z0-9]/.test(p)) score++;
    return score;
  }

  strengthColor(): string {
    return ['transparent', '#c62828', '#f57f17', '#f4b400', '#2e7d32'][this.passwordStrength()];
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.errorMessage.set('');
    const v = this.form.getRawValue();
    this.auth.register({
      email: v.email,
      password: v.password,
      displayName: v.displayName || undefined
    }).subscribe({
      next: () => {
        const ref = this.dialog.open(MessageDialog, {
          data: {
            title: '¡Cuenta creada! 🎉',
            message: 'Tu cuenta se creó con éxito. Ahora ingresa con tus datos.'
          }
        });
        ref.afterClosed().subscribe(() => this.router.navigate(['/login']));
      },
      error: (err) => {
        this.errorMessage.set(
          err.status === 409
            ? 'Ese correo ya está registrado.'
            : 'No se pudo crear la cuenta. Revisa los datos.'
        );
      }
    });
  }
}