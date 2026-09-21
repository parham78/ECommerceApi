import { Injectable, computed, signal } from '@angular/core';

interface JwtPayload {
  sub: string;
  email: string;
  role: string | string[];
  exp: number;
}

@Injectable({
  providedIn: 'root',
})
export class AuthSession {
  private readonly storageKey = 'access_token';

  readonly token = signal<string | null>(localStorage.getItem(this.storageKey));

  readonly payload = computed(() => {
    const token = this.token();

    if (!token) {
      return null;
    }

    return this.decodeToken(token);
  });

  readonly isAuthenticated = computed(() => {
    const payload = this.payload();

    if (!payload) {
      return false;
    }

    return payload.exp * 1000 > Date.now();
  });

  readonly email = computed(() => this.payload()?.email ?? null);

  readonly roles = computed(() => {
    const role = this.payload()?.role;

    if (!role) {
      return [];
    }

    return Array.isArray(role) ? role : [role];
  });

  setToken(token: string): void {
    localStorage.setItem(this.storageKey, token);
    this.token.set(token);
  }

  clear(): void {
    localStorage.removeItem(this.storageKey);
    this.token.set(null);
  }

  private decodeToken(token: string): JwtPayload | null {
    try {
      const payloadPart = token.split('.')[1];

      if (!payloadPart) {
        return null;
      }

      const normalized = payloadPart.replace(/-/g, '+').replace(/_/g, '/');

      const json = decodeURIComponent(
        atob(normalized)
          .split('')
          .map((character) => '%' + character.charCodeAt(0).toString(16).padStart(2, '0'))
          .join(''),
      );

      return JSON.parse(json) as JwtPayload;
    } catch {
      return null;
    }
  }
}
