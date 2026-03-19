import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DoctorService, Doctor } from '../../../core/services/doctor.service';

@Component({
  selector: 'app-doctor-admin',
  templateUrl: './doctor-admin.component.html',
  styleUrls: ['./doctor-admin.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class DoctorAdminComponent implements OnInit {

  doctors: Doctor[] = [];

  newDoctor = {
    name: '',
    specialization: '',
    email: '',
    phone: ''
  };

  constructor(private doctorService: DoctorService) { }

  ngOnInit(): void {
    this.loadDoctors();
  }

  loadDoctors(): void {
    this.doctorService.getDoctors()
      .subscribe((data: Doctor[]) => this.doctors = data);
  }

  addDoctor(): void {
    this.doctorService.createDoctor(this.newDoctor)
      .subscribe(() => {
        this.loadDoctors();
        this.resetForm();
      });
  }

  deleteDoctor(id: string): void {
    this.doctorService.deleteDoctor(id)
      .subscribe(() => this.loadDoctors());
  }

  updateDoctor(doctor: Doctor): void {
    this.doctorService.updateDoctor(doctor).subscribe(() => {
      this.loadDoctors();
    });
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
