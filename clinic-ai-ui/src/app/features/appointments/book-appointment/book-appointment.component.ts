import { Component ,OnInit} from '@angular/core';
import { AppointmentService } from '../../../core/services/appointment.service';
import { DoctorService, Doctor } from '../../../core/services/doctor.service';

@Component({
  selector: 'app-book-appointment',
  templateUrl: './book-appointment.component.html'
   
})
export class BookAppointmentComponent implements OnInit{

  doctors: Doctor[] = [];
  model = {
     doctorId: '',
    patientId: '',
    appointmentDate: '',
    startTime: '',
    endTime: '',
    notes: ''
  };

  constructor(
    private doctorService: DoctorService,
    private appointmentService: AppointmentService
  ) {}

  ngOnInit(): void {
    this.doctorService.getDoctors().subscribe((data: Doctor[]) => {
      this.doctors = data;
      });
    }

  submit(){
    this.appointmentService.createAppointment(this.model).subscribe(response => {
      next:()=>{
        alert('Appointment booked successfully!');
      }
      error:()=>{
        alert('Failed to book appointment. Please try again.');
        }
    });
}
}
