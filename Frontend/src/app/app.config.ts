import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay, withNoHttpTransferCache } from '@angular/platform-browser';
import { authInterceptor } from './core/interceptors/interceptors';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes), provideClientHydration(withEventReplay()),
    // provideHttpClient(
    //   //withInterceptors([authInterceptor]), // Wired up for every request here
    //   withFetch(),
    // ),
    provideClientHydration(
      withNoHttpTransferCache()
    )
  ]
};
