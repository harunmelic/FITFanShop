import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  UserDto,
  CreateUserCommand,
  UpdateUserCommand,
  UpdateUserPasswordCommand
} from './user-api.model';

@Injectable({
  providedIn: 'root'
})
export class UserApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/Users`;
  private http = inject(HttpClient);

  /**
   * GET /api/users
   * Get all users
   */
  getAll(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(this.apiUrl);
  }

  /**
   * GET /api/users/{id}
   * Get user by ID
   */
  getById(id: number): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/${id}`);
  }

  /**
   * POST /api/users
   * Create a new user
   */
  create(payload: CreateUserCommand): Observable<UserDto> {
    return this.http.post<UserDto>(this.apiUrl, payload);
  }

  /**
   * PUT /api/users/{id}
   * Update an existing user
   */
  update(id: number, payload: UpdateUserCommand): Observable<UserDto> {
    return this.http.patch<UserDto>(`${this.apiUrl}/${id}`, payload);
  }

  /**
   * DELETE /api/users/{id}
   * Delete a user
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * PUT /api/users/{id}/password
   * Update user password
   */
  updatePassword(id: number, payload: UpdateUserPasswordCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/password`, payload);
  }
}
