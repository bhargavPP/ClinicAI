import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentService } from "../../core/services/appointment.service";
@Component({
  selector: 'app-appointment-model',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './appointment-model.html'
})
export class AppointmentModel {

  @Input() appointment: any;
  @Output() closeEvent = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<string>();
  close() {
    this.closeEvent.emit();
  }
  constructor(private appointmentService: AppointmentService) { }
  cancel(a: any) {

    if (!confirm('Are you sure you want to cancel this appointment?')) return;

    this.appointmentService.cancelAppointment(a.id).subscribe({
      next: () => {

        // ✅ update list UI
        a.status = 'Cancelled';

        this.cancelled.emit(a.id);
      },
      error: (err) => {
        console.error('Cancel failed', err);
      }
    });
  }
}
