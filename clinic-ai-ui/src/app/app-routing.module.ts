import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DoctorListComponent } from './features/doctors/doctor-list/doctor-list.component';
import { BookAppointmentComponent } from './features/appointments/book-appointment/book-appointment.component';
const routes: Routes = [{ path: 'doctors', component: DoctorListComponent },
                        { path: 'book', component: BookAppointmentComponent },
                        { path: '', redirectTo: 'doctors', pathMatch: 'full' }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
