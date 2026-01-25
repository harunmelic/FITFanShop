import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  LoginCommand,
  LoginCommandDto,
  RefreshTokenCommand,
  RefreshTokenCommandDto,
  LogoutCommand,
  RegisterCommand,
  GetSecurityQuestionResponse,
  VerifySecurityAnswerCommand,
  ResetPasswordCommand
} from './auth-api.model';

@Injectable({
  providedIn: 'root'
})
export class AuthApiService {
  private readonly baseUrl = `${environment.apiUrl}/api/auth`;
  private http = inject(HttpClient);

  /**
   * POST /Auth/login
   * Authenticate user and get access/refresh tokens.
   */
  login(payload: LoginCommand): Observable<LoginCommandDto> {
    return this.http.post<LoginCommandDto>(`${this.baseUrl}/login`, payload);
  }

  /**
   * POST /Auth/register
   * Register new user and get access/refresh tokens.
   */
  register(payload: RegisterCommand): Observable<LoginCommandDto> {
    return this.http.post<LoginCommandDto>(`${this.baseUrl}/register`, payload);
  }

  /**
   * POST /Auth/refresh
   * Refresh access token using refresh token.
   */
  refresh(payload: RefreshTokenCommand): Observable<RefreshTokenCommandDto> {
    return this.http.post<RefreshTokenCommandDto>(`${this.baseUrl}/refresh`, payload);
  }

  /**
   * POST /Auth/logout
   * Invalidate refresh token and logout user.
   */
  logout(payload: LogoutCommand): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/logout`, payload);
  }

  /**
   * GET /Auth/security-question
   * Get security question for email.
   */
  getSecurityQuestion(email: string): Observable<GetSecurityQuestionResponse> {
    return this.http.get<GetSecurityQuestionResponse>(`${this.baseUrl}/security-question`, {
      params: { email }
    });
  }

  /**
   * POST /Auth/verify-security-answer
   * Verify security answer for password reset.
   */
  verifySecurityAnswer(command: VerifySecurityAnswerCommand): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/verify-security-answer`, command);
  }

  /**
   * POST /Auth/reset-password
   * Reset password using security answer.
   */
  resetPassword(command: ResetPasswordCommand): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/reset-password`, command);
  }
}
