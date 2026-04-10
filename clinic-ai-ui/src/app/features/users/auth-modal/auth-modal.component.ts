import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmService } from '../../../core/services/confirm.service';
type ActiveTab = 'login' | 'signup';

@Component({
  selector: 'app-auth-modal',
  standalone: true,
  templateUrl: './auth-modal.component.html',
  styleUrl: './auth-modal.component.css',
  imports: [
    CommonModule,        //  REQUIRED for *ngIf, *ngFor
    ReactiveFormsModule  //  REQUIRED for formGroup
  ]
})
export class AuthModalComponent {

  @Output() closeModal = new EventEmitter<void>();
  @Output() authSuccess = new EventEmitter<void>();

  activeTab: ActiveTab = 'login';   // ✅ Bug 2 fixed: lowercase
  isLoading = false;
  errorMessage = '';
  showPassword = false;
  showConfirmPassword = false;

  loginForm: FormGroup;
  signupForm: FormGroup;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router, private toast: ToastService, private confirm: ConfirmService) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });

    this.signupForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
    }, { validators: this.passwordMatchValidator });
  }

  switchTab(tab: ActiveTab): void {
    this.activeTab = tab;
    this.errorMessage = '';
    this.loginForm.reset();
    this.signupForm.reset();
  }

  onLogin(): void {
    if (this.loginForm.invalid) { this.loginForm.markAllAsTouched(); return; }
    this.isLoading = true;
    this.errorMessage = '';
    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.toast.show('Login success', 'success');
        this.isLoading = false;
        // this.router.navigate(['/book']);
        this.router.navigateByUrl('/doctors');
      },
      error: (err) => {
        this.isLoading = false; this.errorMessage = err.message || 'Login failed. Please try again.';
        this.toast.show('Login failed :' + err?.error?.message, 'danger')
      }
    });
  }

  onSignup(): void {
    if (this.signupForm.invalid) { this.signupForm.markAllAsTouched(); return; }
    this.isLoading = true;
    this.errorMessage = '';
    const { confirmPassword, ...payload } = this.signupForm.value;
    this.authService.register(this.signupForm.value).subscribe({
      next: () => {
        this.isLoading = false; this.router.navigate(['/book']);
        this.toast.show('User Registered successfully', 'success');
      },
      error: (err) => {
        this.isLoading = false; this.errorMessage = err?.error?.message || 'Registration failed. Please try again.';
        this.toast.show('Registration failed :' + err?.error?.message, 'danger')
      }
    });
  }

  isInvalid(form: FormGroup, field: string): boolean {
    const ctrl = form.get(field);
    return !!(ctrl?.invalid && ctrl?.touched);
  }

  private passwordMatchValidator(group: AbstractControl) {
    const pass = group.get('password')?.value;
    const confirm = group.get('confirmPassword')?.value;
    return pass === confirm ? null : { passwordMismatch: true };
  }
}
