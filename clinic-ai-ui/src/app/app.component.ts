import { Component, inject   } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastComponent } from "./shared/toast/toast.component";
import { ConfirmComponent } from './shared/confirm/confirm.component';
import { CommonModule } from '@angular/common';
import { AuthService } from '././core/services/auth.service';
import { AuthModalComponent } from './features/users/auth-modal/auth-modal.component';
import { Router } from '@angular/router';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [RouterModule, ToastComponent, ConfirmComponent, CommonModule, AuthModalComponent]
})
export class AppComponent {
  title = 'clinic`';

  isSidebarOpen = true;
  showAuthModal = false;
  router = inject(Router);
  auth = inject(AuthService);
  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }
  onAuthSuccess() {
    this.showAuthModal = false;
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
