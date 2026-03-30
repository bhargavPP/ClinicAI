import { Component, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

export interface Doctor {
  name: string;
  specialization: string;
  department: string;
  experience: string;
  bio: string;
  image: string;
  languages: string[];
}

@Component({
  selector: 'app-doctors',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './doctors.component.html',
  styleUrls: ['./doctors.component.css']
})
export class DoctorsComponent implements AfterViewInit, OnDestroy {

  activeDept = 'All';

  departments = ['All', 'Cardiology', 'Neurology', 'Orthopedics', 'Ophthalmology', 'General Medicine'];

  doctors: Doctor[] = [
    {
      name: 'Dr. James Harrington', specialization: 'Interventional Cardiologist', department: 'Cardiology',
      experience: '18 years', bio: 'Dr. Harrington specializes in complex coronary interventions and preventive cardiology, with fellowship training from the Cleveland Clinic.',
      image: 'https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?w=400&q=80',
      languages: ['English', 'French']
    },
    {
      name: 'Dr. Aisha Patel', specialization: 'Consultant Neurologist', department: 'Neurology',
      experience: '14 years', bio: 'Dr. Patel is an expert in epilepsy management and headache disorders, with a special interest in neuro-immunology.',
      image: 'https://images.unsplash.com/photo-1594824476967-48c8b964273f?w=400&q=80',
      languages: ['English', 'Hindi', 'Gujarati']
    },
    {
      name: 'Dr. Marcus Linton', specialization: 'Orthopedic Surgeon', department: 'Orthopedics',
      experience: '11 years', bio: 'Dr. Linton focuses on sports medicine and minimally invasive joint surgery, helping athletes return to peak performance.',
      image: 'https://images.unsplash.com/photo-1537368910025-700350fe46c7?w=400&q=80',
      languages: ['English']
    },
    {
      name: 'Dr. Sophia Chen', specialization: 'Family Physician', department: 'General Medicine',
      experience: '9 years', bio: 'Dr. Chen believes in whole-person care and long-term patient relationships, with a focus on preventive medicine and women\'s health.',
      image: 'https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=400&q=80',
      languages: ['English', 'Mandarin', 'Cantonese']
    },
    {
      name: 'Dr. Raj Mehta', specialization: 'Ophthalmologist', department: 'Ophthalmology',
      experience: '12 years', bio: 'Dr. Mehta has performed over 2,000 cataract surgeries and is a leading expert in glaucoma management and laser vision correction.',
      image: 'https://images.unsplash.com/photo-1582750433449-648ed127bb54?w=400&q=80',
      languages: ['English', 'Hindi', 'Punjabi']
    },
    {
      name: 'Dr. Emily Walsh', specialization: 'Cardiologist', department: 'Cardiology',
      experience: '8 years', bio: 'Dr. Walsh specializes in women\'s cardiac health and non-invasive imaging, with expertise in echocardiography.',
      image: 'https://images.unsplash.com/photo-1651008376811-b90baee60c1f?w=400&q=80',
      languages: ['English']
    }
  ];

  whyUs = [
    { icon: '🏅', title: 'Board Certified', description: 'Every physician at Meridian holds recognized board certification in their specialty.' },
    { icon: '🌐', title: 'Multilingual Team', description: 'We serve Brampton\'s diverse community in over 8 languages for a comfortable experience.' },
    { icon: '🤝', title: 'Collaborative Care', description: 'Our specialists work as a team, sharing insights to ensure the best outcomes for you.' },
    { icon: '📱', title: 'Always Accessible', description: 'Reach your care team through our patient portal for questions between appointments.' }
  ];

  private observer!: IntersectionObserver;

  constructor(private router: Router) { }

  get filteredDoctors(): Doctor[] {
    return this.activeDept === 'All'
      ? this.doctors
      : this.doctors.filter(d => d.department === this.activeDept);
  }

  ngAfterViewInit(): void {
    this.initReveal();
  }

  ngOnDestroy(): void {
    if (this.observer) this.observer.disconnect();
  }

  setDept(dept: string): void {
    this.activeDept = dept;
    setTimeout(() => this.initReveal(), 50);
  }

  bookDoctor(doctor: Doctor): void {
    this.router.navigate(['/contact'], { queryParams: { doctor: doctor.name } });
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
