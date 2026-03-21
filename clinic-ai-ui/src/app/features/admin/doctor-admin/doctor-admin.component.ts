import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DoctorService, Doctor } from '../../../core/services/doctor.service';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmService } from '../../../core/services/confirm.service';

@Component({
  selector: 'app-doctor-admin',
  templateUrl: './doctor-admin.component.html',
  styleUrls: ['./doctor-admin.component.css'],
 
  imports: [CommonModule, FormsModule]
})
export class DoctorAdminComponent implements OnInit {
  errors: String[] = [];

  doctors: Doctor[] = [];
  editDoctorId: string | null = null;
  editModel: Partial<Doctor> = {};
  validationErrors: string[] = [];

  newDoctor = {
    name: '',
    specialization: '',
    email: '',
    phone: ''
  };

  constructor(private doctorService: DoctorService, private toast: ToastService,
    private confirmService: ConfirmService) { }

  ngOnInit(): void {
    this.loadDoctors();
  }

  loadDoctors(): void {
    this.doctorService.getDoctors().subscribe({
      next: (data: Doctor[]) => {
     
        this.doctors = [...data];
        
      },
      error: (err) => {
        console.error(' Error loading doctors:', err);
        this.toast.show('loading failed :' + err, 'danger')
      },
      complete: () => {
        console.log(' API call completed');
      }
    });
  }

  addDoctor(): void {
    
    // validate before creating
    this.validationErrors = this.validateDoctor(this.newDoctor as Partial<Doctor>);
    if (this.validationErrors.length > 0) {
      
      return;
    }
    
    this.doctorService.createDoctor(this.newDoctor)
      .subscribe({
        next: () => {
          this.toast.show('Created successfully', 'success');
          this.loadDoctors();
          this.resetForm();
           
        },
        error: (errors: string[]) => {
          this.toast.show('Update failed :' + errors, 'danger')
          this.validationErrors = errors;  
        }
      });
  }

  deleteDoctor(id: string): void {
    this.confirmService.confirm('Delete this record?')
      .then(result => {
        if (!result) return;

        this.doctorService.deleteDoctor(id)
          .subscribe(() => {
            this.toast.show('Deleted successfully', 'success');
            this.loadDoctors();
          });
      });
     
  }

  updateDoctor(doctor: Doctor): void {
    // client-side validation before sending update
    this.validationErrors = this.validateDoctor(doctor);
    if (this.validationErrors.length > 0) {
      return;
    }

    this.doctorService.updateDoctor(doctor).subscribe({
      next: () => {
        this.toast.show('Updated successfully', 'success');
        this.loadDoctors();
        this.cancelEdit();
      },
      error: (err) => {
        this.toast.show(' failed :' + err, 'danger')
        // show server-side validation errors if present
        this.validationErrors = [err?.error || 'Failed to update doctor'];
      }
    });
  }

  beginEdit(doctor: Doctor) {
    this.editDoctorId = doctor.id;
    // create a shallow copy to edit
    this.editModel = { ...doctor };
    this.validationErrors = [];
  }

  saveEdit() {
    if (!this.editDoctorId) return;
    const updated: Doctor = {
      id: this.editDoctorId,
      name: this.editModel.name || '',
      specialization: this.editModel.specialization || '',
      email: this.editModel.email || '',
      phone: this.editModel.phone || ''
    };
    this.updateDoctor(updated);
  }

  cancelEdit() {
    this.editDoctorId = null;
    this.editModel = {};
    this.validationErrors = [];
  }

  private validateDoctor(doctor: Partial<Doctor>): string[] {
    const errors: string[] = [];
    if (!doctor.name || doctor.name.trim().length === 0) {
      errors.push('Name is required');
    } else if (doctor.name.length > 100) {
      errors.push('Name must be 100 characters or less');
    }
    if (!doctor.specialization || doctor.specialization.trim().length === 0) {
      errors.push('Specialization is required');
    } else if (doctor.specialization.length > 100) {
      errors.push('Specialization must be 100 characters or less');
    }
    if (!doctor.email || doctor.email.trim().length === 0) {
      errors.push('Email is required');
    } else if (!/^\S+@\S+\.\S+$/.test(doctor.email)) {
      errors.push('Email is not valid');
    } else if ((doctor.email || '').length > 100) {
      errors.push('Email must be 100 characters or less');
    }
    if (!doctor.phone || doctor.phone.trim().length === 0) {
      errors.push('Phone is required');
    } else if ((doctor.phone || '').length > 100) {
      errors.push('Phone must be 100 characters or less');
    }
    return errors;
  }

  resetForm(): void {
     this.newDoctor = {
      name: '',
      specialization: '',
      email: '',
      phone: ''
    };
  }
} 
