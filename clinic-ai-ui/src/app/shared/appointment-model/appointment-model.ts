import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-appointment-model',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './appointment-model.html'
})
export class AppointmentModel {

  @Input() appointment: any;
  @Output() closeEvent = new EventEmitter<void>();

  close() {
    this.closeEvent.emit();
  }

  cancel() {
    console.log('Cancel clicked');
  }
}
