import { NgModule, provideZoneChangeDetection } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DoctorListComponent } from './features/doctors/doctor-list/doctor-list.component';
import { BookAppointmentComponent } from './features/appointments/book-appointment/book-appointment.component';
import { FormsModule } from '@angular/forms';
import { DoctorAdminComponent } from './features/admin/doctor-admin/doctor-admin.component';

import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ErrorInterceptor } from './core/interceptors/error.interceptor';
import { DoctorAvailabilityComponent } from './features/availability/doctor-availability/doctor-availability.component';
import { ToastComponent } from './shared/toast/toast.component';
import { ConfirmComponent } from './shared/confirm/confirm.component';
@NgModule({
  declarations: [ ],
  imports: [
    BrowserModule,
    CommonModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    // If components are standalone, import them here instead of declaring
    AppComponent,
    DoctorListComponent,
    BookAppointmentComponent,
    DoctorAdminComponent,
    DoctorAvailabilityComponent,
    ToastComponent, ConfirmComponent
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }, provideZoneChangeDetection({ eventCoalescing: true })
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
