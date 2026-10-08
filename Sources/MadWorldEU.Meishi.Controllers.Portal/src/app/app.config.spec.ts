import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { appConfig } from './app.config';
import { routes } from './app.routes';

describe('appConfig', () => {
  it('should provide the router with the application routes', () => {
    TestBed.configureTestingModule({ providers: appConfig.providers });

    expect(TestBed.inject(Router).config).toEqual(routes);
  });
});
