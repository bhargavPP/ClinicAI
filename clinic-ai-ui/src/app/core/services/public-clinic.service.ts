import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PublicClinicService {

  getClinic() {
    return of({
      name: 'My Clinic',
      tagline: 'Your Trusted Family Healthcare Partner',
      description: 'We provide high-quality healthcare services with experienced doctors and modern facilities.',

      heroImage: 'https://img.freepik.com/free-photo/doctor-with-stethoscope-hands-hospital-background_1423-1.jpg',

      services: [
        {
          slug: 'general-consultation',
          title: 'General Consultation',
          icon: '🩺',
          description: 'Complete health checkups and diagnosis.',
          image: 'https://img.freepik.com/free-photo/doctor-consulting-patient_23-2148827773.jpg'
        },
        {
          slug: 'dental-care',
          title: 'Dental Care',
          icon: '🦷',
          description: 'Professional dental treatments and care.',
          image: 'https://img.freepik.com/free-photo/dentist-treating-patient_23-2148984912.jpg'
        },
        { title: 'Physiotherapy', icon: '💪' },
        { title: 'Vaccination', icon: '💉' }
      ],

      doctors: [
        {
          name: 'Dr. John Doe',
          specialization: 'General Physician',
          experience: '10+ Years',
          image: 'https://randomuser.me/api/portraits/men/32.jpg'
        },
        {
          name: 'Dr. Smith',
          specialization: 'Dentist',
          experience: '8+ Years',
          image: 'https://randomuser.me/api/portraits/women/44.jpg'
        }
      ],

      contact: {
        phone: '+1 123 456 7890',
        email: 'info@clinic.com',
        address: 'Brampton, ON'
      }
    });
  }
}
