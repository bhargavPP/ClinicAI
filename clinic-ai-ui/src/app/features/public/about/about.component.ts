import { Component, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

interface TimelineItem {
  year: string;
  title: string;
  description: string;
}

interface Value {
  icon: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css']
})
export class AboutComponent implements AfterViewInit, OnDestroy {

  timeline: TimelineItem[] = [
    { year: '2014', title: 'Founded', description: 'Meridian Health opened its doors as a small family practice in Brampton, ON, with 3 doctors and a vision.' },
    { year: '2016', title: 'First Expansion', description: 'We expanded our facility to include cardiology and orthopedic specialists, doubling our capacity.' },
    { year: '2018', title: 'Lab & Diagnostics', description: 'Launched our in-house diagnostic laboratory, enabling same-day test results for patients.' },
    { year: '2021', title: 'Digital Health', description: 'Introduced online booking, patient portals, and telehealth consultations for greater accessibility.' },
    { year: '2024', title: 'Today', description: 'Serving 5,000+ patients annually with 15+ specialists across 6 departments — and growing.' }
  ];

  values: Value[] = [
    { icon: '🤝', title: 'Integrity', description: 'We are transparent, honest, and accountable in every interaction with our patients and their families.' },
    { icon: '🏅', title: 'Excellence', description: 'We hold ourselves to the highest clinical and service standards, continuously improving our practice.' },
    { icon: '❤️', title: 'Compassion', description: 'We treat every patient as a whole person, not just a diagnosis — with empathy at every touchpoint.' },
    { icon: '💡', title: 'Innovation', description: 'We embrace new technologies and methods that improve outcomes and make care more accessible.' },
    { icon: '🌍', title: 'Community', description: 'We are deeply rooted in Brampton and committed to the wellbeing of the community we serve.' },
    { icon: '🔒', title: 'Privacy', description: 'We protect every patient\'s data and dignity with the utmost care and confidentiality.' }
  ];

  private observer!: IntersectionObserver;

  constructor(private router: Router) { }

  ngAfterViewInit(): void {
    this.observer = new IntersectionObserver(
      (entries) => entries.forEach(e => {
        if (e.isIntersecting) { e.target.classList.add('visible'); this.observer.unobserve(e.target); }
      }),
      { threshold: 0.12 }
    );
    document.querySelectorAll('.reveal').forEach(el => this.observer.observe(el));
  }

  ngOnDestroy(): void {
    if (this.observer) this.observer.disconnect();
  }

  goToContact(): void {
    this.router.navigate(['/contact']);
  }
}
