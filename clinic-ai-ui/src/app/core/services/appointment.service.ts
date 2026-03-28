import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CreateAppointment {
  doctorId: string;
  patientId: string;
  appointmentDate: string;
  startTime: string;
  //endTime: string;
  notes?: string;
}
@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private apiUrl = `${environment.apiUrl}/appointments`;
  constructor(private http: HttpClient) {
  }
  createAppointment(data: CreateAppointment): Observable<any> {
    return this.http.post<any>(this.apiUrl, data, {
      withCredentials: true
    });
  }
  getSlots(doctorId: string, date: string): Observable<any> {

    return this.http.get<string[]>(`${this.apiUrl}/slots?doctorId=${doctorId}&date=${date}`, {
      withCredentials: true
    });
  }
  getAppointments(patientId: string) {
  
    let url = this.apiUrl;

    if (patientId) {
      url = `${this.apiUrl}?patientId=${patientId}`;
    }
    return this.http.get<any[]>(url, {
      withCredentials: true
    });
  }
  getMyAppointments() {
    return this.http.get<any[]>(`${this.apiUrl}/my-appointments`, {
      withCredentials: true
    });
  }
  updateAppointment(data: any) {
    return this.http.put<any>(`${this.apiUrl}/${data.id}`, data);
  }
  cancelAppointment(id: string) {
    return this.http.put<any>(`${this.apiUrl}/${id}/cancel`, {}, {
      withCredentials: true
    });
  }
}
