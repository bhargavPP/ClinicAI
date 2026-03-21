import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {

  private resolver!: (value: boolean) => void;

  confirm(message: string): Promise<boolean> {
    const event = new CustomEvent('app-confirm', { detail: message });
    window.dispatchEvent(event);

    return new Promise<boolean>((resolve) => {
      this.resolver = resolve;
    });
  }

  resolve(value: boolean) {
    this.resolver(value);
  }
}
