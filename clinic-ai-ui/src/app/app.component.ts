import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastComponent } from "./shared/toast/toast.component";
import { ConfirmComponent } from './shared/confirm/confirm.component';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [RouterModule, ToastComponent, ConfirmComponent, CommonModule]
})
export class AppComponent {
  title = 'clinic-ai-ui';

  isSidebarOpen = true;

  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }
}
