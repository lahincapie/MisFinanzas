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

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}