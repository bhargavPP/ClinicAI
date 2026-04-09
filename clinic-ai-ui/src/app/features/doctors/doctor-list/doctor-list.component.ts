import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DoctorService, Doctor } from '../../../core/services/doctor.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-doctor-list',
  templateUrl: './doctor-list.component.html',
  standalone: true,
  imports: [CommonModule]
})
export class DoctorListComponent implements OnInit {

  doctors: Doctor[] = [];
  error = false;

  constructor(private doctorService: DoctorService, private auth: AuthService) { }

  ngOnInit(): void {
    if (!this.auth.currentUser()) return; // ← bail if not authenticated

    this.doctorService.getDoctors().subscribe({
      next: (data: Doctor[]) => this.doctors = data,
      error: () => this.error = true
    });
  }
}
