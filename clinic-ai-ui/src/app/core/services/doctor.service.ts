import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import  {environment} from '../../../environments/environment';
import { Observable } from 'rxjs';

export interface Doctor{
  id:string;
  name:string;
  specialization:string;
  email:string;
  phone:string;
}

@Injectable({
  providedIn: 'root'
})

export class DoctorService {

  private apiUrl = `${environment.apiUrl}/doctors`;
  constructor(private http:HttpClient) {
  }
  getDoctors(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(this.apiUrl);
    }
  createDoctor(data: any) {
    return this.http.post(this.apiUrl, data);
  }
  deleteDoctor(id: string) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
  updateDoctor(doctor: Doctor) {
    return this.http.put(`${this.apiUrl}/${doctor.id}`, doctor) 
  }
}
