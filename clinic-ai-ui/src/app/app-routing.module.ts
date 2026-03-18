import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DoctorListComponent } from './features/doctors/doctor-list/doctor-list.component';
const routes: Routes = [{ path: 'doctors', component: DoctorListComponent },
                        { path: '', redirectTo: 'doctors', pathMatch: 'full' }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
