import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css',
})
export class LoginPage {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  correo = '';
  password = '';

  cargando = false;
  error = '';

  iniciarSesion(): void {
    this.error = '';

    if (!this.correo.trim() || !this.password) {
      this.error = 'Ingresa tu correo y contraseña.';
      return;
    }

    this.cargando = true;

    this.authService
      .login({
        correo: this.correo.trim(),
        password: this.password,
      })
      .subscribe({
        next: () => {
          this.cargando = false;
          this.router.navigate(['/panel']);
        },
        error: (error: HttpErrorResponse) => {
          this.cargando = false;

          this.error =
            error.error?.mensaje ??
            'No se pudo iniciar sesión. Verifica tus credenciales.';
        },
      });
  }
}