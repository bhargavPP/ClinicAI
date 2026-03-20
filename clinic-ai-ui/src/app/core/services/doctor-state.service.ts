import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Doctor, DoctorService } from './doctor.service';

@Injectable({ providedIn: 'root' })
export class DoctorStateService {
  private doctorsSubject = new BehaviorSubject<Doctor[]>([]);
  public doctors$ = this.doctorsSubject.asObservable();

  constructor(private doctorService: DoctorService) {}

  refresh(): void {
    this.doctorService.getDoctors().subscribe({
      next: (data) => this.doctorsSubject.next(data),
      error: (err) => console.error('Failed to refresh doctors', err)
    });
  }

  // Optional polling helper
  startPolling(intervalMs: number = 5000): () => void {
    const sub = setInterval(() => this.refresh(), intervalMs);
    // return stop function
    return () => clearInterval(sub);
  }
}
