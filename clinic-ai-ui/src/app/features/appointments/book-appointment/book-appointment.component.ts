import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { AppointmentService } from '../../../core/services/appointment.service';
import { DoctorService, Doctor } from '../../../core/services/doctor.service';
import { PatientService } from '../../../core/services/patient.service';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmService } from '../../../core/services/confirm.service';
@Component({
  selector: 'app-book-appointment',
  templateUrl: './book-appointment.component.html',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class BookAppointmentComponent implements OnInit {

  form!: FormGroup;

  today: string = '';
  doctors: Doctor[] = [];
  patients: any[] = [];
  slots: string[] = [];

  appointments: any[] = [];
  currentPatientId: string = '';

  editMode = false;
  editingAppointmentId = '';

  constructor(
    private fb: FormBuilder,
    private doctorService: DoctorService,
    private appointmentService: AppointmentService,
    private patientService: PatientService,
    private toast: ToastService,
    private confirmService: ConfirmService
  ) { }

  ngOnInit(): void {

    this.today = this.formatDate(new Date());

    this.form = this.fb.group({
      doctorId: [''],
      patientId: [null], // ✅ IMPORTANT (object binding)
      patientName: [{ value: '', disabled: true }],
      email: [{ value: '', disabled: true }],
      phone: [{ value: '', disabled: true }],
      dateOfBirth: [{ value: '', disabled: true }],
      appointmentDate: [''],
      startTime: [''],
      notes: ['']
    });
  
    this.doctorService.getDoctors()
      .subscribe(d => this.doctors = d);

    this.patientService.getPatient()
      .subscribe(p => this.patients = p);

    // ✅ Reactive patient handling
    this.form.get('patientId')?.valueChanges.subscribe((p: any) => {

      if (!p || !p.id) {
        console.log('skipping invalid request');
        return;
      }
      if (this.currentPatientId === p.id) return;
      this.currentPatientId = p.id;

      this.form.patchValue({
        patientName: p.name,
        email: p.email,
        phone: p.phone,
        dateOfBirth: p.dateOfBirth?.split('T')[0]
      }, { emitEvent: false });

     
    });

    this.loadAppointments();
  }

  resetForm() {
    this.form.reset({}, {emitEvent:false});
    this.editMode = false;
    this.editingAppointmentId = '';
    this.slots = [];
  }

  loadSlots() {

    const doctorId = this.form.value.doctorId;
    const date = this.form.value.appointmentDate;

    if (!doctorId || !date) return;

    this.appointmentService.getSlots(doctorId, date)
      .subscribe({
        next: res => this.slots = res,
        error: err => this.toast.show(err.error, 'danger')
      });
  }

  cancelAppointment(id: string) {

    this.confirmService.confirm('Are you sure you want to cancel this appointment?').then(result => {
     
      if (!result) return;


      this.appointmentService.cancelAppointment(id).subscribe({
        next: (res) => {
        
          if (!res.isSuccess) {
            this.toast.show(res.message, 'danger');
            return;
          }
          else {
            this.toast.show(res.message, 'success');
            this.loadAppointments();
          }
        },
        error: (err) => {
         // const msg = err?.error?.message || err?.error || err?.message || 'Something went wrong';
          this.toast.show(err, 'danger'); console.log(err);

        }
      });
    });
  }

  submit() {

    const patient = this.form.value.patientId;

    if (!patient) {
      this.toast.show('Select patient', 'danger');
      return;
    }

    const payload: any = {
      doctorId: this.form.value.doctorId,
      patientId: patient.id, // ✅ extract ID
      appointmentDate: this.form.value.appointmentDate,
      startTime: this.form.value.startTime,
      notes: this.form.value.notes
    };

    if (this.editMode) {
      payload.id = this.editingAppointmentId;
   
      this.appointmentService.updateAppointment(payload).subscribe({
        next: (res) => {
         
        if (!res.isSuccess) {
            this.toast.show(res.message, 'danger');
            return;
          }

          this.afterSave(res.message);
         // this.loadAppointments();
        },

        error: (err) => {
          let msg = 'Something went wrong';

          if (err.error) {
            if (Array.isArray(err.error)) {
              msg = err.error.join(', ');
            } else if (typeof err.error === 'string') {
              msg = err.error;
            } else if (err.error.message) {
              msg = err.error.message;
            } else if (err.error.errors) {
              msg = Object.values(err.error.errors).flat().join(', ');
            }
          }

          this.toast.show(msg, 'danger');
        }
      });

      return;
    }

    this.appointmentService.createAppointment(payload).subscribe({
      next: (resp) => {
        if (!resp.isSuccess) {
          this.toast.show(resp.message, 'danger');
          return;
        }

        this.afterSave(resp.message);
        //this.loadAppointments();
      },

        error: (err) => {
          let msg = 'Something went wrong';

          if (err.error) {
            if (Array.isArray(err.error)) {
              msg = err.error.join(', ');
            } else if (typeof err.error === 'string') {
              msg = err.error;
            } else if (err.error.message) {
              msg = err.error.message;
            } else if (err.error.errors) {
              msg = Object.values(err.error.errors).flat().join(', ');
            }
          }

          this.toast.show(msg, 'danger');
        }
    });

    return;
     
  }

  editAppointment(a: any) {

    this.editMode = true;
    this.editingAppointmentId = a.id;

    const formattedDate = a.date.split('T')[0];
    const formattedDob = a.dateOfBirth?.split('T')[0];
    const formattedTime = a.startTime.substring(0, 5);
 
    const selectedPatient = this.patients.find(p => p.id.toLowerCase() === a.patientId.toLowerCase());

    if (!selectedPatient) {
      console.error('Patient not found for id:', a.patientId);
      return;
    }

    this.currentPatientId = selectedPatient.id;
    this.form.patchValue({
      doctorId: a.doctorId,
      patientId: selectedPatient, // ✅ FIX
      patientName: a.patientName,
      email: a.email,
      phone: a.phone,
      dateOfBirth: formattedDob,
      appointmentDate: formattedDate,
      startTime: '',
      notes: a.notes
    });

    this.loadSlots();

    setTimeout(() => {
      this.form.patchValue({ startTime: formattedTime });
    }, 100);
  }

  loadAppointments() {
    
     this.appointmentService
      .getAppointments(this.currentPatientId)
       .subscribe(res => {
        
         this.appointments = res;
       });
  }

  afterSave(msg: string) {
    this.toast.show(msg , 'success');
  //  this.form.reset();
    this.editMode = false;
    this.slots = [];
     this.loadAppointments();
    this.form.reset({}, {emitEvent:false});
  }

  formatTime(time: string): string {
    return time.substring(0, 5);
  }

  isPast(a: any): boolean {
    return new Date(a.date) < new Date(new Date().toDateString());
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}
