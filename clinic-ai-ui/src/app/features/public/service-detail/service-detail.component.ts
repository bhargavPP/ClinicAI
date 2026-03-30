import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PublicClinicService } from '../../../core/services/public-clinic.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-service-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './service-detail.component.html',
  styleUrls: ['./service-detail.component.css']
})
export class ServiceDetailComponent implements OnInit {

  service: any;

  constructor(
    private route: ActivatedRoute,
    private clinicService: PublicClinicService
  ) { }

  ngOnInit(): void {
    const slug = this.route.snapshot.paramMap.get('serviceSlug');

    this.clinicService.getClinic().subscribe(res => {
      this.service = res.services.find((s: any) => s.slug === slug);
    });
  }
}
