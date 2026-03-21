import { Component, OnInit,    } from '@angular/core';
import { DoctorService } from '../../../core/services/doctor.service';
import { AvailabilityService } from '../../../core/services/availability.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmService } from '../../../core/services/confirm.service';
@Component({
  selector: 'app-doctor-availability',
  templateUrl: './doctor-availability.component.html',

  imports: [CommonModule, FormsModule]
})
export class DoctorAvailabilityComponent implements OnInit {

  doctors: any[] = [];

  model = {
    doctorId: '',
    date: '',
    startTime: '',
    endTime: ''
  };
  filters: any = {
    doctorId: '',
    fromDate: '',
    toDate: ''
  }
  availabilities: any[] = [];
  editMode = false;
  editingId: string | null = null;

  validationErrors: string[] = [];

  constructor(
    private doctorService: DoctorService,
    private availabilityService: AvailabilityService,
    private toast: ToastService,
    private confirmService: ConfirmService
  ) { }

  loadAvailabilities() {
    this.availabilityService.getAvailability(this.filters).subscribe(
      {
        next: (res) => {
          this.availabilities = res;
        },
        error: (err) => {
          //console.error('ERROR:', err);
          this.toast.show('Update failed:' + err, 'danger');
        }
      });
  }

  ngOnInit(): void {
    this.doctorService.getDoctors()
      .subscribe(res => this.doctors = res);

    this.loadAvailabilities();
  }

  save() {

    this.validationErrors = this.validateAvailability(this.model);
    if (this.validationErrors.length > 0) return;

    const payload = {
      id: this.editingId,
      ...this.model,
      startTime: this.model.startTime + ':00',
      endTime: this.model.endTime + ':00'
    };

    // ✅ UPDATE MODE
    if (this.editMode && this.editingId) {
      this.availabilityService.updateAvailability(this.editingId, payload)
        .subscribe({
          next: () => {
            this.toast.show('Updated successfully', 'success');
            this.resetForm();
            this.loadAvailabilities();
          },
          error: (err) => this.toast.show('Update failed :'+err, 'danger')
        });

    } else {
      // ✅ CREATE MODE
      this.availabilityService.createAvailability(payload)
        .subscribe({
          next: () => {
            this.toast.show('Created successfully', 'success');
            this.resetForm();
            this.loadAvailabilities();
             
          },
          error: (err) => {
            if (err.status === 400 && err.error?.errors) {
              this.validationErrors = Object.values(err.error.errors).flat() as string[];
            } else {
              this.toast.show('Update failed', 'danger');
            }
          }
        });
      
    }
  }
  // ✅ FIX: normalize time (remove seconds for input type="time")
  edit(a: any) {
    this.editMode = true;
    this.editingId = a.id;

    this.model = {
      doctorId: a.doctorId,
      date: a.date ? a.date.split('T')[0] : '',
      startTime: a.startTime?.substring(0, 5),
      endTime: a.endTime?.substring(0, 5)
    };
  }

  delete(id: string) {
    this.confirmService.confirm('Delete this record?')
      .then(result => {
        if (!result) return;

        this.availabilityService.deleteAvailability(id)
          .subscribe(() => {
            this.toast.show('Deleted successfully', 'success');
            this.loadAvailabilities();
          });
      });
  }
  // ✅ correct validation
  private validateAvailability(model: any): string[] {
    const errors: string[] = [];

    if (!model.doctorId) {
      errors.push('Doctor is required');
    }

    if (!model.date) {
      errors.push('Date is required');
    }

    if (!model.startTime) {
      errors.push('Start time is required');
    }

    if (!model.endTime) {
      errors.push('End time is required');
    }

    if (model.startTime && model.endTime && model.startTime >= model.endTime) {
      errors.push('Start time must be before end time');
    }

    return errors;
  }

  resetForm(): void {
    this.model = {
      doctorId: '',
      date: '',
      startTime: '',
      endTime: ''
    };

    this.editMode = false;
    this.editingId = null;
    this.validationErrors = [];
  }
}
