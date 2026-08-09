import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatIconModule],
  templateUrl: './layout.html',
  styleUrl: './layout.css'
})
export class Layout {
  private auth = inject(AuthService);
  private router = inject(Router);

  userEmail = this.auth.getUserEmail();
  userName = this.auth.getUserName() || this.buildNameFromEmail();

  private buildNameFromEmail(): string {
    const email = this.auth.getUserEmail();
    if (!email) return 'Usuario';
    const raw = email.split('@')[0];
    return raw.charAt(0).toUpperCase() + raw.slice(1);
  }

   logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}