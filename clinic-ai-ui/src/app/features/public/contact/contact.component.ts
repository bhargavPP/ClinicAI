import { Component, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Hour { day: string; time: string; closed: boolean; }
interface AppointmentForm {
  firstName: string; lastName: string; email: string;
  phone: string; department: string; date: string; message: string;
}

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements AfterViewInit, OnDestroy {

  submitted = false;
  today = new Date().toISOString().split('T')[0];

  departments = ['Cardiology', 'Neurology', 'Orthopedics', 'Ophthalmology', 'General Medicine', 'Laboratory'];

  form: AppointmentForm = {
    firstName: '', lastName: '', email: '',
    phone: '', department: '', date: '', message: ''
  };

  hours: Hour[] = [
    { day: 'Monday', time: '9:00 AM – 7:00 PM', closed: false },
    { day: 'Tuesday', time: '9:00 AM – 7:00 PM', closed: false },
    { day: 'Wednesday', time: '9:00 AM – 7:00 PM', closed: false },
    { day: 'Thursday', time: '9:00 AM – 7:00 PM', closed: false },
    { day: 'Friday', time: '9:00 AM – 7:00 PM', closed: false },
    { day: 'Saturday', time: '10:00 AM – 4:00 PM', closed: false },
    { day: 'Sunday', time: '', closed: true }
  ];

  private observer!: IntersectionObserver;

  ngAfterViewInit(): void {
    this.observer = new IntersectionObserver(
      (entries) => entries.forEach(e => {
        if (e.isIntersecting) { e.target.classList.add('visible'); this.observer.unobserve(e.target); }
      }),
      { threshold: 0.1 }
    );
    document.querySelectorAll('.reveal').forEach(el => this.observer.observe(el));
  }

  ngOnDestroy(): void {
    if (this.observer) this.observer.disconnect();
  }

  onSubmit(): void {
    // Replace with your actual API call / service
    console.log('Appointment request:', this.form);
    this.submitted = true;
  }

  reset(): void {
    this.submitted = false;
    this.form = { firstName: '', lastName: '', email: '', phone: '', department: '', date: '', message: '' };
  }
}
