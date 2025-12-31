import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app/app.routes';

import { provideAuth0, authHttpInterceptorFn } from '@auth0/auth0-angular';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authHttpInterceptorFn])),

    provideAuth0({
      domain: 'dev-ic6hqtbqegunpxep.eu.auth0.com',
      clientId: 'ouc1jkBVAiGSPIaVihuUsX2nXJPa8hzF',
      authorizationParams: {
        redirect_uri: window.location.origin,
        audience: 'https://job-search-assistant-api'
      },
      httpInterceptor: {
        allowedList: [
          {
            uri: '/api/*',
            tokenOptions: {
              authorizationParams: {
                audience: 'https://job-search-assistant-api'
              }
            }
          }
        ]
      }
    })
  ]
}).catch((err) => console.error(err));
