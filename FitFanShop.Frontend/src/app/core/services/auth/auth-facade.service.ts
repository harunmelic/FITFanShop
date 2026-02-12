// src/app/core/services/auth/auth-facade.service.ts
import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of, tap, catchError, map } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

import { AuthApiService } from '../../../api-services/auth/auth-api.service';
import {
  LoginCommand,
  LoginCommandDto,
  LogoutCommand,
  RefreshTokenCommand,
  RefreshTokenCommandDto,
  RegisterCommand,
  GetSecurityQuestionResponse,
  VerifySecurityAnswerCommand,
  VerifySecurityAnswerResponse,
  ResetPasswordCommand,
} from '../../../api-services/auth/auth-api.model';

import { AuthStorageService } from './auth-storage.service';
import { CurrentUserDto } from './current-user.dto';
import { JwtPayloadDto } from './jwt-payload.dto';

/**
 * Main auth service (façade).
 * - Communicates with AuthApiService (HTTP)
 * - Communicates with AuthStorageService (localStorage)
 * - Decodes JWT and holds CurrentUser as signal
 *
 * Used in:
 * - interceptor (getAccessToken, refresh)
 * - guards (isAuthenticated, isAdmin)
 * - components (login, logout, navbar)
 */
@Injectable({ providedIn: 'root' })
export class AuthFacadeService {
  private api = inject(AuthApiService);
  private storage = inject(AuthStorageService);
  private router = inject(Router);

  // === REACTIVE STATE: current user ===

  private _currentUser = signal<CurrentUserDto | null>(null);

  /** readonly signal for UI – read as auth.currentUser() */
  currentUser = this._currentUser.asReadonly();

  /** computed signals over current user */
  isAuthenticated = computed(() => !!this._currentUser());
  isAdmin = computed(() => this._currentUser()?.isAdmin ?? false);
  isManager = computed(() => this._currentUser()?.isManager ?? false);
  isEmployee = computed(() => this._currentUser()?.isEmployee ?? false);

  constructor() {
    // Attempt initialization from existing access token
    this.initializeFromToken();
  }

  // =========================================================
  // PUBLIC API
  // =========================================================

  /**
   * User login (email + password).
   * Saves tokens to storage, decodes JWT and populates current user state.
   */
  login(payload: LoginCommand): Observable<void> {
    return this.api.login(payload).pipe(
      tap((response: LoginCommandDto) => {
        this.storage.saveLogin(response);           // access + refresh + expiries
        this.decodeAndSetUser(response.accessToken); // populate _currentUser
      }),
      map(() => void 0)
    );
  }

  /**
   * Register novog korisnika.
   * Saves tokens to storage, decodes JWT and populates current user state.
   */
  register(payload: RegisterCommand): Observable<void> {
    return this.api.register(payload).pipe(
      tap((response: LoginCommandDto) => {
        this.storage.saveLogin(response);           // access + refresh + expiries
        this.decodeAndSetUser(response.accessToken); // populate _currentUser
      }),
      map(() => void 0)
    );
  }

  /**
   * Get security question for email.
   */
  getSecurityQuestion(email: string): Observable<GetSecurityQuestionResponse> {
    return this.api.getSecurityQuestion(email);
  }

  /**
   * Verify security answer for password reset.
   */
  verifySecurityAnswer(email: string, securityAnswer: string): Observable<VerifySecurityAnswerResponse> {
    return this.api.verifySecurityAnswer({ email, securityAnswer });
  }

  /**
   * Reset password using security answer.
   */
  resetPassword(email: string, resetToken: string, newPassword: string, confirmNewPassword: string): Observable<void> {
    return this.api.resetPassword({ email, resetToken, newPassword, confirmNewPassword });
  }

  /**
   * Logout user:
   * - Clear state and tokens locally
   * - Try to invalidate refresh token on server (ignore errors)
   */
  logout(): Observable<void> {
    const refreshToken = this.storage.getRefreshToken();

    // 1) Clear locally (optimistic logout)
    this.clearUserState();

    // 2) No refresh token → no API call
    if (!refreshToken) {
      return of(void 0);
    }

    const payload: LogoutCommand = { refreshToken };

    // 3) Attempt server-side logout, ignore errors
    return this.api.logout(payload).pipe(catchError(() => of(void 0)));
  }

  /**
   * Refresh access token – uses refresh token.
   * Called by interceptor when receiving 401.
   */
  refresh(payload: RefreshTokenCommand): Observable<RefreshTokenCommandDto> {
    return this.api.refresh(payload).pipe(
      tap((response: RefreshTokenCommandDto) => {
        this.storage.saveRefresh(response);           // save new tokens
        this.decodeAndSetUser(response.accessToken);  // update current user
      })
    );
  }

  /**
   * Utility for guards/interceptors – clear auth state and redirect to /login.
   */
  redirectToLogin(): void {
    this.clearUserState();
    this.router.navigate(['/auth/login']);
  }

  // =========================================================
  // GETTERI ZA INTERCEPTOR
  // =========================================================

  /**
   * Access token za Authorization header.
   */
  getAccessToken(): string | null {
    return this.storage.getAccessToken();
  }

  /**
   * Refresh token za refresh poziv.
   */
  getRefreshToken(): string | null {
    return this.storage.getRefreshToken();
  }

  updateCurrentUserName(firstName: string, lastName: string): void {
    const current = this._currentUser();
    if (!current) {
      return;
    }

    this._currentUser.set({
      ...current,
      firstName,
      lastName
    });
  }

  // =========================================================
  // PRIVATE HELPERS
  // =========================================================

  /**
   * On app start (constructor) – try to restore state from existing token.
   */
  private initializeFromToken(): void {
    const token = this.storage.getAccessToken();
    if (token) {
      this.decodeAndSetUser(token);
    }
  }

  /**
   * Decode JWT and set current user state.
   */
  private decodeAndSetUser(token: string): void {
    try {
      const payload = jwtDecode<JwtPayloadDto>(token);

      const user: CurrentUserDto = {
        userId: Number(payload.sub),
        email: payload.email,
        firstName: payload.given_name,
        lastName: payload.family_name,
        isAdmin: payload.is_admin === 'true',
        isManager: payload.is_manager === 'true',
        isEmployee: payload.is_employee === 'true',
        isMember: payload.is_member === 'true', // Premium membership
        tokenVersion: Number(payload.ver),
      };

      this._currentUser.set(user);
    } catch (error) {
      console.error('Failed to decode JWT token:', error);
      this._currentUser.set(null);
    }
  }

  /**
   * Clear user state + all tokens from storage.
   */
  private clearUserState(): void {
    this._currentUser.set(null);
    this.storage.clear();
  }
}
