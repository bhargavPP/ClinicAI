import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DoctorListComponent } from './features/doctors/doctor-list/doctor-list.component';
import { BookAppointmentComponent } from './features/appointments/book-appointment/book-appointment.component';
import { DoctorAdminComponent } from './features/admin/doctor-admin/doctor-admin.component';
import { DoctorAvailabilityComponent } from './features/availability/doctor-availability/doctor-availability.component';
import { authGuard } from './core/guard/auth-guard';
import { roleGuard } from './core/guard/role-guard';           // ← new guard (step 1)
import { loginGuard } from './core/guard/login-guard';         // ← new guard (step 2)
import { AuthModalComponent } from './features/users/auth-modal/auth-modal.component';
import { DoctorCalendarComponent } from './features/doctor/doctor-calendar/doctor-calendar.component';
import { PatientComponent } from './features/patient/patient/patient.component';
const routes: Routes = [
  // Login page — redirect away if already logged in
  { path: 'login', component: AuthModalComponent, canActivate: [loginGuard] },
  // Protected routes
  { path: 'doctors', component: DoctorListComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Patient'] } },
  { path: 'book', component: BookAppointmentComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Patient'] } },
  { path: 'admin', component: DoctorAdminComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] } },
  { path: 'availability', component: DoctorAvailabilityComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Doctor'] } },
  { path: 'calendar', component: DoctorCalendarComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Doctor'] } },
  { path: 'patient', component: PatientComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Patient'] } }, 
  // Default redirect
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
