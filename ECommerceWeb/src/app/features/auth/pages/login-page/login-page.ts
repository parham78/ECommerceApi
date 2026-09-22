import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { AuthApi } from '../../data-access/auth-api';
import { AuthSession } from '../../data-access/auth-session';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
})
export class LoginPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authApi = inject(AuthApi);
  private readonly authSession = inject(AuthSession);
  private readonly router = inject(Router);

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    this.authApi.login(this.form.getRawValue()).subscribe({
      next: (response) => {
        this.authSession.setToken(response.token);
        this.isSubmitting.set(false);

        if (this.authSession.roles().includes('Admin')) {
          void this.router.navigate(['/admin']);
          return;
        }

        void this.router.navigate(['/products']);
      },

      error: (error: HttpErrorResponse) => {
        this.isSubmitting.set(false);

        if (error.status === 401) {
          this.errorMessage.set('Invalid email or password.');
          return;
        }

        this.errorMessage.set('We could not sign you in. Please try again.');
      },
    });
  }
}
