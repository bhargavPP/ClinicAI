import { Component, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

export interface Service {
  slug: string;
  icon: string;
  title: string;
  description: string;
  tags: string[];
  detailTitle: string;
  detailDescription: string;
  points: string[];
  image: string;
}

interface Step {
  num: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-service-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './service-detail.component.html',
  styleUrls: ['./service-detail.component.css']
})
export class ServiceDetailComponent implements AfterViewInit, OnDestroy {

  selectedService: Service | null = null;

  services: Service[] = [
    {
      slug: 'cardiology', icon: '🫀', title: 'Cardiology',
      description: 'Comprehensive heart health services including advanced diagnostics, monitoring, and preventive programs.',
      tags: ['ECG', 'Echo', 'Stress Test'],
      detailTitle: 'Expert Heart Care You Can Trust',
      detailDescription: 'Our cardiology department offers a full range of diagnostic and therapeutic services for heart conditions. From routine cardiac screening to complex arrhythmia management, our team provides personalized, evidence-based care.',
      points: ['12-Lead ECG & Holter Monitoring', 'Echocardiography', 'Stress Testing', 'Preventive Cardiology Programs', 'Lipid & Blood Pressure Management'],
      image: 'https://images.unsplash.com/photo-1628348068343-c6a848d2b6dd?w=700&q=80'
    },
    {
      slug: 'neurology', icon: '🧠', title: 'Neurology',
      description: 'Expert care for conditions of the brain, spine, and nervous system with advanced imaging and diagnostics.',
      tags: ['EEG', 'EMG', 'MRI Review'],
      detailTitle: 'Advanced Neurological Care',
      detailDescription: 'Our neurology team specializes in diagnosing and treating a wide range of neurological disorders, from migraines and epilepsy to complex movement disorders. We combine clinical excellence with cutting-edge diagnostic technology.',
      points: ['EEG & EMG Studies', 'Headache & Migraine Management', 'Epilepsy Care', 'Neuropathy Assessment', 'Stroke Risk Evaluation'],
      image: 'https://images.unsplash.com/photo-1559757175-5700dde675bc?w=700&q=80'
    },
    {
      slug: 'orthopedics', icon: '🦴', title: 'Orthopedics',
      description: 'Specialized care for musculoskeletal conditions, sports injuries, joint pain, and rehabilitation.',
      tags: ['Joint Care', 'Sports Injuries', 'Rehab'],
      detailTitle: 'Restore Movement, Rebuild Strength',
      detailDescription: 'Whether you\'re recovering from a sports injury or managing chronic joint pain, our orthopedic specialists provide tailored treatment plans focused on restoring your mobility and quality of life.',
      points: ['Joint Pain Assessment & Treatment', 'Sports Injury Management', 'Fracture Care', 'Physiotherapy Integration', 'Post-Surgical Rehabilitation'],
      image: 'https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=700&q=80'
    },
    {
      slug: 'ophthalmology', icon: '👁️', title: 'Ophthalmology',
      description: 'Complete eye care from routine exams to LASIK consultations and surgical interventions.',
      tags: ['Eye Exams', 'LASIK', 'Glaucoma'],
      detailTitle: 'Clear Vision, Expert Care',
      detailDescription: 'Our ophthalmology team provides comprehensive eye care for patients of all ages. From basic vision correction to complex retinal conditions, we are equipped to handle every aspect of your eye health.',
      points: ['Comprehensive Eye Exams', 'Prescription & Contact Lens Fitting', 'Glaucoma Screening & Management', 'Cataract Evaluation', 'LASIK Consultation'],
      image: 'https://images.unsplash.com/photo-1576091160550-2173dba999ef?w=700&q=80'
    },
    {
      slug: 'general', icon: '🩺', title: 'General Medicine',
      description: 'Preventive care, annual checkups, chronic disease management, and acute illness treatment.',
      tags: ['Checkups', 'Chronic Care', 'Immunizations'],
      detailTitle: 'Your Family\'s Health Partner',
      detailDescription: 'Our general medicine physicians are your first line of care. We build long-term relationships with patients and families, managing everything from routine wellness visits to complex chronic conditions.',
      points: ['Annual Physical Exams', 'Chronic Disease Management (Diabetes, Hypertension)', 'Immunizations & Travel Health', 'Acute Illness Treatment', 'Health Screening & Prevention'],
      image: 'https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=700&q=80'
    },
    {
      slug: 'laboratory', icon: '🧬', title: 'Laboratory',
      description: 'In-house diagnostics and pathology with same-day turnaround and precise, reliable results.',
      tags: ['Blood Work', 'Pathology', 'Same-Day'],
      detailTitle: 'Precision Diagnostics, Fast Results',
      detailDescription: 'Our state-of-the-art on-site laboratory processes hundreds of tests daily with rapid turnaround. From routine bloodwork to specialized panels, our certified lab technologists ensure accuracy and reliability.',
      points: ['Complete Blood Count & Metabolic Panels', 'Lipid & Thyroid Profiles', 'Urinalysis & Microbiology', 'Hormone & Vitamin Testing', 'Same-Day Results Available'],
      image: 'https://images.unsplash.com/photo-1582719471384-894fbb16e074?w=700&q=80'
    }
  ];

  steps: Step[] = [
    { num: '01', title: 'Book Online or Call', description: 'Schedule in minutes through our online system or by calling our patient coordinators directly.' },
    { num: '02', title: 'See Your Specialist', description: 'Meet with a board-certified specialist who takes the time to understand your full health picture.' },
    { num: '03', title: 'Get Your Care Plan', description: 'Receive a clear, personalized care plan with next steps, prescriptions, or referrals as needed.' },
    { num: '04', title: 'Follow-Up & Support', description: 'We follow up after every visit to ensure your recovery is on track and answer any questions.' }
  ];

  private observer!: IntersectionObserver;

  constructor(private router: Router) { }

  ngAfterViewInit(): void {
    this.initReveal();
  }

  ngOnDestroy(): void {
    if (this.observer) this.observer.disconnect();
  }

  selectService(service: Service): void {
    this.selectedService = this.selectedService?.slug === service.slug ? null : service;
    setTimeout(() => this.initReveal(), 50);
  }

  goToContact(): void {
    this.router.navigate(['/contact']);
  }

  private initReveal(): void {
    if (this.observer) this.observer.disconnect();
    this.observer = new IntersectionObserver(
      (entries) => entries.forEach(e => {
        if (e.isIntersecting) { e.target.classList.add('visible'); this.observer.unobserve(e.target); }
      }),
      { threshold: 0.1 }
    );
    document.querySelectorAll('.reveal').forEach(el => this.observer.observe(el));
  }
}
