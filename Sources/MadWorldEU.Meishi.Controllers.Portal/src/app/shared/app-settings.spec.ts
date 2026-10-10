import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AppSettings, DEFAULT_API_URL, SETTINGS_URL } from './app-settings';

describe('AppSettings', () => {
  let settings: AppSettings;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    settings = TestBed.inject(AppSettings);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('should use the default API url before the settings are loaded', () => {
    expect(settings.apiUrl).toBe(DEFAULT_API_URL);
  });

  it('should use the API url from config.json', async () => {
    const loaded = settings.load();
    http.expectOne(SETTINGS_URL).flush({ apiUrl: 'https://api.example.com' });
    await loaded;

    expect(settings.apiUrl).toBe('https://api.example.com');
  });

  it('should strip trailing slashes from the API url', async () => {
    const loaded = settings.load();
    http.expectOne(SETTINGS_URL).flush({ apiUrl: 'https://api.example.com/' });
    await loaded;

    expect(settings.apiUrl).toBe('https://api.example.com');
  });

  it.each([{ apiUrl: '' }, {}, null])(
    'should fall back to the default when config.json contains %j',
    async (body) => {
      const loaded = settings.load();
      http.expectOne(SETTINGS_URL).flush(body);
      await loaded;

      expect(settings.apiUrl).toBe(DEFAULT_API_URL);
    },
  );

  it('should fall back to the default when config.json cannot be loaded', async () => {
    const loaded = settings.load();
    http.expectOne(SETTINGS_URL).flush('', { status: 404, statusText: 'Not Found' });
    await loaded;

    expect(settings.apiUrl).toBe(DEFAULT_API_URL);
  });
});
