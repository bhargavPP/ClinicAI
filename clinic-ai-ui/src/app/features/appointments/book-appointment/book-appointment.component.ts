import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppointmentService } from '../../../core/services/appointment.service';
import { DoctorService, Doctor } from '../../../core/services/doctor.service';
import { PatientService } from '../../../core/services/patient.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-book-appointment',
  templateUrl: './book-appointment.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class BookAppointmentComponent implements OnInit {

  today: string = '';
  doctors: Doctor[] = [];
  slots: string[] = [];

  appointments: any[] = [];
  currentPatientId: string = '';

  editMode: boolean = false;
  editingAppointmentId: string = '';

  model = {
    doctorId: '',
    patientName: '',
    email: '',
    phone: '',
    dateOfBirth: '',
    appointmentDate: '',
    startTime: '',
    notes: ''
  };

  constructor(
    private doctorService: DoctorService,
    private appointmentService: AppointmentService,
    private patientService: PatientService,
    private toast: ToastService
  ) { }

  ngOnInit(): void {
    this.today = this.formatDate(new Date());

    this.doctorService.getDoctors().subscribe((data: Doctor[]) => {
      this.doctors = data;
    });

    this.loadAppointments();
  }

  // 🔹 Load slots
  loadSlots() {
    if (!this.model.doctorId || !this.model.appointmentDate) return;

    this.appointmentService
      .getSlots(this.model.doctorId, this.model.appointmentDate)
      .subscribe({
        next: (res: string[]) => {
          this.slots = res;
        },
        error: err => {
          this.toast.show(err.error, 'danger');
        }
      });
  }

  // 🔹 Submit (Create / Update)
  submit() {

    if (!this.model.startTime) {
      this.toast.show('Please select a slot', 'danger');
      return;
    }

    // ✅ EDIT MODE (UPDATED FOR RESULT PATTERN)
    if (this.editMode) {

      const appointment = {
        id: this.editingAppointmentId,
        doctorId: this.model.doctorId,
        appointmentDate: this.model.appointmentDate,
        startTime: this.model.startTime,
        notes: this.model.notes
      };

      this.appointmentService.updateAppointment(appointment).subscribe({
        next: (res) => {
          if (!res.success) {
            this.toast.show(res.message, 'danger');
            return;
          }

          this.toast.show(res.message, 'success');
          this.afterSave();
        },
        error: () => this.toast.show('Something went wrong', 'danger')
      });

      return;
    }

    // ✅ CREATE MODE
    const patient = {
      name: this.model.patientName,
      email: this.model.email,
      phone: this.model.phone,
      dateOfBirth: this.model.dateOfBirth
    };

    this.patientService.createPatient(patient).subscribe({
      next: (patientId) => {

        this.currentPatientId = patientId;

        const appointment = {
          doctorId: this.model.doctorId,
          patientId: patientId,
          appointmentDate: this.model.appointmentDate,
          startTime: this.model.startTime,
          notes: this.model.notes
        };

        this.appointmentService.createAppointment(appointment).subscribe({
          next: (res) => {
            if (!res.success) {
              this.toast.show(res.message, 'danger');
              return;
            }

            this.toast.show(res.message, 'success');
            this.afterSave();
          },
          error: () => this.toast.show('Something went wrong', 'danger')
        });
      },
      error: () => this.toast.show('Failed to create patient', 'danger')
    });
  }

  // 🔹 Edit appointment
  editAppointment(a: any) {
    this.editMode = true;
    this.editingAppointmentId = a.id;

    const formattedDate = a.date ? a.date.split('T')[0] : '';
    const formattedDob = a.dateOfBirth
      ? a.dateOfBirth.split('T')[0]
      : '';
    const formattedTime = a.startTime?.substring(0, 5);

    this.model = {
      doctorId: a.doctorId?.toString(),
      patientName: a.patientName || '',
      email: a.email || '',
      phone: a.phone || '',
      dateOfBirth: formattedDob,              // ✅ FIXED
      appointmentDate: formattedDate,
      startTime: '',
      notes: a.notes || ''
    };

    this.loadSlots();

    // ensure dropdown matches slot
    setTimeout(() => {
      this.model.startTime = formattedTime;
    }, 100);
  }

  // 🔹 Load appointments (FIXED)
  loadAppointments() {
     

    this.appointmentService
      .getAppointments(this.currentPatientId)
      .subscribe(res => this.appointments = res);
  }

  // 🔹 After save
  afterSave() {
    this.slots = [];
    this.resetForm();
    this.loadAppointments();
  }

  // 🔹 Reset form
  resetForm(): void {
    this.model = {
      doctorId: '',
      patientName: '',
      email: '',
      phone: '',
      dateOfBirth: '',
      appointmentDate: '',
      startTime: '',
      notes: ''
    };

    this.editMode = false;
    this.editingAppointmentId = '';
  }

  // 🔹 Cancel appointment (UPDATED FOR RESULT PATTERN)
  cancelAppointment(id: string) {

    if (!confirm('Are you sure you want to cancel this appointment?')) {
      return;
    }

    this.appointmentService.cancelAppointment(id).subscribe({
      next: (res) => {
        if (!res.success) {
          this.toast.show(res.message, 'danger');
          return;
        }

        this.toast.show(res.message, 'success');
        this.loadAppointments();
      },
      error: () => this.toast.show('Something went wrong', 'danger')
    });
  }

  // 🔹 Helpers
  isPast(a: any): boolean {
    const today = new Date();
    const apptDate = new Date(a.date);
    return apptDate < new Date(today.toDateString());
  }

  formatTime(time: string): string {
    return time.substring(0, 5);
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}
