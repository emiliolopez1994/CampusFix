import { Routes } from '@angular/router';
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'panel' },
  ...['panel', 'reportes', 'tablero', 'personas'].map((path) => ({
    path,
    loadComponent: () =>
      import('./features/reportes/pages/reportes.page').then((m) => m.ReportesPage),
    data: { vista: path },
  })),
  { path: '**', redirectTo: 'panel' },
];
