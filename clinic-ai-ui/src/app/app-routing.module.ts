import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DoctorListComponent } from './features/doctors/doctor-list/doctor-list.component';
import { BookAppointmentComponent } from './features/appointments/book-appointment/book-appointment.component';
import { DoctorAdminComponent } from './features/admin/doctor-admin/doctor-admin.component';
import { DoctorAvailabilityComponent } from './features/availability/doctor-availability/doctor-availability.component';
import { DoctorCalendarComponent } from './features/doctor/doctor-calendar/doctor-calendar.component';
import { PatientComponent } from './features/patient/patient/patient.component';
import { AuthModalComponent } from './features/users/auth-modal/auth-modal.component';

import { authGuard } from './core/guard/auth-guard';
import { roleGuard } from './core/guard/role-guard';
import { loginGuard } from './core/guard/login-guard';

const routes: Routes = [

  // ✅ PUBLIC ROUTE (keep before redirects)
  {
    path: 'clinic/:slug',
    loadComponent: () =>
      import('./features/public/clinic-public/clinic-public.component')
        .then(m => m.ClinicPublicComponent)
  },
  {
    path: 'clinic/:clinicSlug/services/:serviceSlug',
    loadComponent: () =>
      import('./features/public/service-detail/service-detail.component')
        .then(m => m.ServiceDetailComponent)
  },
  // Login page
  {
    path: 'login',
    component: AuthModalComponent,
    canActivate: [loginGuard]
  },

  // Protected routes
  {
    path: 'doctors',
    component: DoctorListComponent,
    canActivate: [authGuard] 
  },
  {
    path: 'book',
    component: BookAppointmentComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Patient'] }
  },
  {
    path: 'admin',
    component: DoctorAdminComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'] }
  },
  {
    path: 'availability',
    component: DoctorAvailabilityComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Doctor'] }
  },
  {
    path: 'calendar',
    component: DoctorCalendarComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Doctor'] }
  },
  {
    path: 'patient',
    component: PatientComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Patient'] }
  },
  { path: 'about', loadComponent: () => import('./features/public/about/about.component').then(m => m.AboutComponent) },
  { path: 'services', loadComponent: () => import('./features/public/service-detail/service-detail.component').then(m => m.ServiceDetailComponent) },
  { path: 'doctorservice', loadComponent: () => import('./features/public/doctors/doctors.component').then(m => m.DoctorsComponent) },
  { path: 'contact', loadComponent: () => import('./features/public/contact/contact.component').then(m => m.ContactComponent) },
  // Default redirect
  { path: '', redirectTo: 'clinic/myClinic', pathMatch: 'full' },

  // ❗ MUST BE LAST
  { path: '**', redirectTo: 'clinic/myClinic' },
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./features/public/unauthorized/unauthorized.component')
        .then(m => m.UnauthorizedComponent)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
