import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastComponent } from "./shared/toast/toast.component";
import { ConfirmComponent } from './shared/confirm/confirm.component';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [RouterModule, ToastComponent, ConfirmComponent]
})
export class AppComponent {
  title = 'clinic-ai-ui';
}
