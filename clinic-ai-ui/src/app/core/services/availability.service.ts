import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Availability {
  id: string;
  Date: string;
  startTime: string;
  endTime: string;
  IsAvailable: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class AvailabilityService {

  private apiUrl = `${environment.apiUrl}/availability`;

  constructor(private http: HttpClient) { }

  createAvailability(data: any) {
    return this.http.post(this.apiUrl, data);
  }
  //getAvailability(): Observable<Availability[]> {
  //  return this.http.get<Availability[]>(this.apiUrl);
  //}
  // ✅ FIX: add filters support
  getAvailability(filters: any): Observable<Availability[]> {
    let params: any = {};

    if (filters.doctorId) params.doctorId = filters.doctorId;
    if (filters.fromDate) params.fromDate = filters.fromDate;
    if (filters.toDate) params.toDate = filters.toDate;

    return this.http.get<Availability[]>(this.apiUrl, { params });
  }
  deleteAvailability(id: string) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
  updateAvailability(id: string, data: any) {
    return this.http.put(`${this.apiUrl}/${id}`, data);
  }
}
