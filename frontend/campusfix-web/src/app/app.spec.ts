import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
describe('CampusFix shell', () => {
  it('muestra la navegación principal', async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter([])],
    }).compileComponents();
    const f = TestBed.createComponent(App);
    await f.whenStable();
    expect(f.nativeElement.textContent).toContain('CampusFix');
    expect(f.nativeElement.querySelectorAll('nav a').length).toBe(4);
  });
});
