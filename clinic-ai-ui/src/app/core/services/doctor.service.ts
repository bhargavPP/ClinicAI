import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import  {environment} from '../../../environments/environment';
import { Observable } from 'rxjs';

export interface Doctor{
  id:string;
  name:string;
  specilization:string;
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
}
