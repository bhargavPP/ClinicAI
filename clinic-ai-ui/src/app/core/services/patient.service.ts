import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import  {environment} from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  private apiUrl = `${environment.apiUrl}/patients`;

  constructor(private http:HttpClient) { }

  createPatient(data: any): Observable<any> {
    return this.http.post(this.apiUrl, data);
    }
}
