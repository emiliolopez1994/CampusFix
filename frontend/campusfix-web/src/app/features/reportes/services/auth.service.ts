import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
  correo: string;
  password: string;
}

export interface RegistroRequest {
  nombre: string;
  correo: string;
  password: string;
}

export interface AuthResponse {
  id: number;
  nombre: string;
  correo: string;
  roles: string[];
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = '/api/auth';

  login(datos: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/login`, datos)
      .pipe(
        tap((respuesta) => {
          this.guardarSesion(respuesta);
        }),
      );
  }

  registro(datos: RegistroRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/registro`, datos)
      .pipe(
        tap((respuesta) => {
          this.guardarSesion(respuesta);
        }),
      );
  }

  private guardarSesion(respuesta: AuthResponse): void {
    localStorage.setItem('campusfix_token', respuesta.token);
    localStorage.setItem(
      'campusfix_usuario',
      JSON.stringify({
        id: respuesta.id,
        nombre: respuesta.nombre,
        correo: respuesta.correo,
        roles: respuesta.roles,
      }),
    );
  }

  obtenerToken(): string | null {
    return localStorage.getItem('campusfix_token');
  }

  estaAutenticado(): boolean {
    return !!this.obtenerToken();
  }

  obtenerUsuario(): Omit<AuthResponse, 'token'> | null {
    const usuario = localStorage.getItem('campusfix_usuario');

    if (!usuario) {
      return null;
    }

    try {
      return JSON.parse(usuario);
    } catch {
      return null;
    }
  }

  tieneRol(rol: string): boolean {
    return this.obtenerUsuario()?.roles.includes(rol) ?? false;
  }

  cerrarSesion(): void {
    localStorage.removeItem('campusfix_token');
    localStorage.removeItem('campusfix_usuario');
  }
}