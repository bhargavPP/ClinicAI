import { Component ,OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppointmentService } from '../../../core/services/appointment.service';
import { DoctorService, Doctor } from '../../../core/services/doctor.service';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-book-appointment',
  templateUrl: './book-appointment.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule]

})
export class BookAppointmentComponent implements OnInit{
  today: string = '';
  doctors: Doctor[] = [];
  model = {
     doctorId: '',
    patientName: '',
  email: '',
  phone: '',
  dateOfBirth: '',
    appointmentDate: '',
    startTime: '',
    endTime: '',
    notes: ''
  };

  constructor(
    private doctorService: DoctorService,
    private appointmentService: AppointmentService,
    private patientService: PatientService
  ) {}

  ngOnInit(): void {
    this.today = this.formatDate(new Date());
  
    this.doctorService.getDoctors().subscribe((data: Doctor[]) => {
      this.doctors = data;
      });
    }

 submit() {

  const patient = {
    name: this.model.patientName,
    email: this.model.email,
    phone: this.model.phone,
    dateOfBirth: this.model.dateOfBirth
  };

  this.patientService.createPatient(patient).subscribe({
    next: (patientId) => {

      const appointment = {
        doctorId: this.model.doctorId,
        patientId: patientId,
        appointmentDate: this.model.appointmentDate,
        startTime: this.model.startTime,
        endTime: this.model.endTime,
        notes: this.model.notes
      };

      this.appointmentService.createAppointment(appointment).subscribe({
        next: () => alert('Appointment booked'),
        error: err => alert(err.error)
      });
    },
    error: err => alert(err.error)
  });


  }
  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}
