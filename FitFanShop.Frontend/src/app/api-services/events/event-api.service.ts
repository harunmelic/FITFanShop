import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EventDetailsDto, EventDto } from './event-api.model';

@Injectable({
  providedIn: 'root'
})
export class EventApiService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/events`;

  getUpcomingEvents(): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(`${this.apiUrl}/upcoming`);
  }

  getById(id: number): Observable<EventDetailsDto> {
    return this.http.get<EventDetailsDto>(`${this.apiUrl}/${id}`);
  }
}
