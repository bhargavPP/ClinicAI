import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface ToastMessage {
  text: string;
  type: 'success' | 'danger' | 'warning' | 'info';
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {

  private toastSubject = new Subject<ToastMessage>();
  toast$ = this.toastSubject.asObservable();

  show(text: string, type: ToastMessage['type'] = 'success') {
    this.toastSubject.next({ text, type });
  }
}
