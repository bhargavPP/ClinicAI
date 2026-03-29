import { Component, OnInit } from '@angular/core';
import { PatientService } from '../../../core/services/patient.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmService } from '../../../core/services/confirm.service';

@Component({
  selector: 'app-patient',
  templateUrl: './patient.component.html',
  imports: [CommonModule, FormsModule]
})
export class PatientComponent implements OnInit {

  model = {
    name: '',
    email: '',
    phone: '',
    dateOfBirth: '',
    relationshipToUser: 'Self'
  };

  patients: any[] = [];

  editMode = false;
  editingId: string | null = null;

  validationErrors: string[] = [];

  constructor(
    private patientService: PatientService,
    private toast: ToastService,
    private confirmService: ConfirmService
  ) { }

  ngOnInit(): void {
    this.loadPatients();
  }

  loadPatients() {
    this.patientService.getPatient().subscribe({
      next: (res) => {
        this.patients = res;
      },
      error: (err) => {
        this.toast.show('Failed to load patients: ' + err, 'danger');
      }
    });
  }

  save() {

    this.validationErrors = this.validatePatient(this.model);
    if (this.validationErrors.length > 0) return;

    const payload = {
      id: this.editingId,
      ...this.model
    };

    // ✅ UPDATE
    if (this.editMode && this.editingId) {
      this.patientService.updatePatient(this.editingId, payload)
        .subscribe({
          next: () => {
            this.toast.show('Updated successfully', 'success');
            this.resetForm();
            this.loadPatients();
          },
          error: (err) => {
            this.toast.show('Update failed: ' + err, 'danger');
          }
        });
    } else {
      // ✅ CREATE
      this.patientService.createPatient(payload)
        .subscribe({
          next: () => {
            this.toast.show('Created successfully', 'success');
            this.resetForm();
            this.loadPatients();
          },
          error: (err) => {
            if (err.status === 400 && err.error?.errors) {
              this.validationErrors = Object.values(err.error.errors).flat() as string[];
            } else {
              this.toast.show(err, 'danger');
            }
          }
        });
    }
  }

  edit(p: any) {
    this.editMode = true;
    this.editingId = p.id;

    this.model = {
      name: p.name,
      email: p.email,
      phone: p.phone,
      dateOfBirth: p.dateOfBirth ? p.dateOfBirth.split('T')[0] : '',
      relationshipToUser: p.relationshipToUser
    };
  }

  delete(id: string, relationship: string) {

    // 🔒 Prevent deleting Self
    if (relationship === 'Self') {
      this.toast.show('You cannot delete yourself', 'warning');
      return;
    }

    this.confirmService.confirm('Delete this patient?')
      .then(result => {
        if (!result) return;

        this.patientService.deletePatient(id)
          .subscribe({
            next: () => {
              this.toast.show('Deleted successfully', 'success');
              this.loadPatients();
            },
            error: (err) => {
              this.toast.show('Delete failed: ' + err, 'danger');
            }
          });
      });
  }

  private validatePatient(model: any): string[] {
    const errors: string[] = [];

    if (!model.name) {
      errors.push('Name is required');
    }

    if (!model.email) {
      errors.push('Email is required');
    }

    if (!model.phone) {
      errors.push('Phone is required');
    }

    if (!model.dateOfBirth) {
      errors.push('Date of birth is required');
    }

    if (!model.relationshipToUser) {
      errors.push('Relationship is required');
    }

    return errors;
  }

  resetForm(): void {
    this.model = {
      name: '',
      email: '',
      phone: '',
      dateOfBirth: '',
      relationshipToUser: 'Self'
    };

    this.editMode = false;
    this.editingId = null;
    this.validationErrors = [];
  }
}
