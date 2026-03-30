import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';          // ← add this
import { RouterModule } from '@angular/router';          // ← add this
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Router } from '@angular/router';
export interface Service {
  slug: string;
  icon: string;
  title: string;
  description: string;
}

export interface Doctor {
  name: string;
  specialization: string;
  experience: string;
  image: string;
}

export interface Clinic {
  name: string;
  tagline: string;
  description: string;
  heroImage: string;
  contact: {
    address: string;
    phone: string;
    email: string;
  };
  services: Service[];
  doctors: Doctor[];
}

export interface WorkingHour {
  day: string;
  hours: string;
  closed: boolean;
}

@Component({
  selector: 'app-clinic-public',
  standalone: true, imports: [CommonModule, RouterModule],
  templateUrl: './clinic-public.component.html',
  styleUrl: './clinic-public.component.css',
})
export class ClinicPublicComponent implements OnInit, AfterViewInit, OnDestroy {
    clinic: Clinic = {
      name: 'Meridian Health',
      tagline: 'World-class medical care delivered with compassion, precision, and respect for every patient.',
      description: 'At Meridian Health, we combine clinical expertise with an unwavering commitment to patient wellbeing — creating an environment where healing begins from the moment you walk through our doors.',
      heroImage: 'https://images.unsplash.com/photo-1519494026892-80bbd2d6fd0d?w=800&q=80',
      contact: {
        address: '150 Main St N, Brampton, ON, Canada',
        phone: '+1 (905) 555-0198',
        email: 'hello@meridianhealth.ca'
      },
      services: [
        { slug: 'cardiology', icon: '🫀', title: 'Cardiology', description: 'Comprehensive heart health services including diagnostics, monitoring, and preventive cardiology programs.' },
        { slug: 'neurology', icon: '🧠', title: 'Neurology', description: 'Expert neurological assessment and treatment for conditions affecting the brain, spine, and nervous system.' },
        { slug: 'orthopedics', icon: '🦴', title: 'Orthopedics', description: 'Specialized care for musculoskeletal conditions, sports injuries, and rehabilitation programs.' },
        { slug: 'ophthalmology', icon: '👁️', title: 'Ophthalmology', description: 'Complete eye care services including routine exams, LASIK consultations, and surgical interventions.' },
        { slug: 'general', icon: '🩺', title: 'General Medicine', description: 'Preventive care, annual checkups, chronic disease management, and acute illness treatment.' },
        { slug: 'laboratory', icon: '🧬', title: 'Laboratory', description: 'In-house diagnostics and pathology with rapid turnaround and precise, reliable results.' }
      ],
      doctors: [
        { name: 'Dr. James Harrington', specialization: 'Cardiology', experience: '18 years of experience', image: 'https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=200&q=80' },
        { name: 'Dr. Aisha Patel', specialization: 'Neurology', experience: '14 years of experience', image: 'https://images.unsplash.com/photo-1594824476967-48c8b964273f?w=200&q=80' },
        { name: 'Dr. Marcus Linton', specialization: 'Orthopedics', experience: '11 years of experience', image: 'https://images.unsplash.com/photo-1537368910025-700350fe46c7?w=200&q=80' },
        { name: 'Dr. Sophia Chen', specialization: 'General Medicine', experience: '9 years of experience', image: 'https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=200&q=80' }
      ]
    };

  workingHours: WorkingHour[] = [
    { day: 'Monday', hours: '9:00 AM – 7:00 PM', closed: false },
    { day: 'Tuesday', hours: '9:00 AM – 7:00 PM', closed: false },
    { day: 'Wednesday', hours: '9:00 AM – 7:00 PM', closed: false },
    { day: 'Thursday', hours: '9:00 AM – 7:00 PM', closed: false },
    { day: 'Friday', hours: '9:00 AM – 7:00 PM', closed: false },
    { day: 'Saturday', hours: '10:00 AM – 4:00 PM', closed: false },
    { day: 'Sunday', hours: '', closed: true }
  ];

  mapUrl!: SafeResourceUrl;

  private observer!: IntersectionObserver;

  constructor(
    private router: Router,
    private sanitizer: DomSanitizer
  ) { }

  ngOnInit(): void {
    const raw = `https://www.google.com/maps?q=${encodeURIComponent(this.clinic.contact.address)}&output=embed`;
    this.mapUrl = this.sanitizer.bypassSecurityTrustResourceUrl(raw);
  }

  ngAfterViewInit(): void {
    this.initScrollReveal();
  }

  ngOnDestroy(): void {
    if (this.observer) {
      this.observer.disconnect();
    }
  }

  goToBooking(): void {
    this.router.navigate(['/login']);
  }

  openService(slug: string): void {
    this.router.navigate(['/services', slug]);
  }

  scrollTo(sectionId: string): void {
    const el = document.getElementById(sectionId);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  }

  private initScrollReveal(): void {
    this.observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add('visible');
            this.observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.12 }
    );

    document.querySelectorAll('.reveal').forEach((el) => {
      this.observer.observe(el);
    });
  }
}
