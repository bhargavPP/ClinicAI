import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmService } from '../../core/services/confirm.service';

@Component({
  selector: 'app-confirm',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirm.component.html'
})
export class ConfirmComponent implements OnInit {

  show = false;
  message = '';

  constructor(private confirmService: ConfirmService) { }

  ngOnInit(): void {
    window.addEventListener('app-confirm', (e: any) => {
      this.message = e.detail;
      this.show = true;
    });
  }

  yes() {
    this.show = false;
    this.confirmService.resolve(true);
  }

  no() {
    this.show = false;
    this.confirmService.resolve(false);
  }
}
