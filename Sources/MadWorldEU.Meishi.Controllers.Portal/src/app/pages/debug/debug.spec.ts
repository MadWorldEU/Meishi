import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { AppSettings } from '../../shared/app-settings';
import { Debug } from './debug';

const PING_URL = 'https://api.example.com/debug/ping';

describe('Debug', () => {
  let fixture: ComponentFixture<Debug>;
  let compiled: HTMLElement;
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Debug],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
    TestBed.inject(AppSettings).apiUrl = 'https://api.example.com';

    fixture = TestBed.createComponent(Debug);
    http = TestBed.inject(HttpTestingController);
    await fixture.whenStable();
    compiled = fixture.nativeElement as HTMLElement;
  });

  afterEach(() => http.verify());

  const button = () => compiled.querySelector<HTMLButtonElement>('button')!;
  const status = () => compiled.querySelector('.result__status');
  const body = () => compiled.querySelector('.result__body');

  const sendPing = async () => {
    button().click();
    await fixture.whenStable();
    return http.expectOne({ method: 'GET', url: PING_URL });
  };

  it('should not send a request or show a result before the button is clicked', () => {
    http.expectNone(PING_URL);
    expect(status()).toBeNull();
    expect(button().disabled).toBe(false);
  });

  it('should show a loading state and disable the button while waiting', async () => {
    const request = await sendPing();

    expect(status()?.textContent).toContain('Waiting for response');
    expect(button().disabled).toBe(true);

    request.flush('pong');
  });

  it('should show the response when the API answers with pong', async () => {
    (await sendPing()).flush('pong');
    await fixture.whenStable();

    expect(status()?.classList).toContain('result__status--success');
    expect(body()?.textContent).toBe('pong');
    expect(button().disabled).toBe(false);
  });

  it('should show an error when the response is not pong', async () => {
    (await sendPing()).flush('<!doctype html>');
    await fixture.whenStable();

    expect(status()?.classList).toContain('result__status--error');
    expect(status()?.textContent).toContain('Unexpected response');
    expect(body()?.textContent).toBe('<!doctype html>');
  });

  it('should show the status when the API returns an error', async () => {
    (await sendPing()).flush('', { status: 502, statusText: 'Bad Gateway' });
    await fixture.whenStable();

    expect(status()?.classList).toContain('result__status--error');
    expect(status()?.textContent).toContain('502 Bad Gateway');
    expect(body()).toBeNull();
    expect(button().disabled).toBe(false);
  });

  it('should show a network error when the API cannot be reached', async () => {
    (await sendPing()).error(new ProgressEvent('error'));
    await fixture.whenStable();

    expect(status()?.textContent).toContain('Network error');
  });

  it('should show the url of the ping request', () => {
    expect(compiled.querySelector('code')?.textContent).toBe(`GET ${PING_URL}`);
  });

  it('should link back to the home page', () => {
    expect(compiled.querySelector('a')?.getAttribute('href')).toBe('/');
  });
});
