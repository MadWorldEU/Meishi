import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { App } from './app';
import { routes } from './app.routes';
import { Debug } from './pages/debug/debug';
import { Home } from './pages/home/home';

describe('App', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideRouter(routes), provideHttpClient(), provideHttpClientTesting()],
    });
  });

  it('should render a router outlet', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();

    expect((fixture.nativeElement as HTMLElement).querySelector('router-outlet')).not.toBeNull();
  });

  it('should show the home page on the root route', async () => {
    const harness = await RouterTestingHarness.create();

    expect(await harness.navigateByUrl('/')).toBeInstanceOf(Home);
  });

  it('should show the debug page on /debug', async () => {
    const harness = await RouterTestingHarness.create();

    expect(await harness.navigateByUrl('/debug')).toBeInstanceOf(Debug);
  });
});
