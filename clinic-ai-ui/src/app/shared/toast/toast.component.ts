import { Component, OnInit } from '@angular/core';
import { ToastService, ToastMessage } from '../../core/services/toast.service';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
  imports: [CommonModule]
})
export class ToastComponent implements OnInit {

  toasts: ToastMessage[] = [];

  constructor(private toastService: ToastService) { }

  ngOnInit(): void {
    this.toastService.toast$.subscribe(msg => {
      this.toasts.push(msg);

      setTimeout(() => {
        this.toasts.shift();
      }, 3000);
    });
  }

  remove(index: number) {
    this.toasts.splice(index, 1);
  }
}
