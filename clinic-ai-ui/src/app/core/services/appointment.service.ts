import {HttpClient} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import  {environment} from '../../../environments/environment';

export interface CreateAppointment {
  doctorId: string;
  patientId:string;
  appointmentDate: string;
  startTime: string;
  endTime: string;
  notes?: string;
  }
@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
    private apiUrl = `${environment.apiUrl}/appointments`;
  constructor(private http:HttpClient) {
  }
  createAppointment(data: CreateAppointment):Observable<any> {
    return this.http.post(this.apiUrl, data);
    }
}
