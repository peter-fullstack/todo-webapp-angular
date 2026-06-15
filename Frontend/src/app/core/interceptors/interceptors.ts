// core/interceptors/auth.interceptor.ts
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { UserService } from '../services/user.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // Inject the shared singleton service directly into the stream pipeline
  const userService = inject(UserService);
  
  // Read the signal value instantly (.currentUserId() evaluates synchronously)
  const currentUserId = userService.currentUser();

  // If a user is selected, clone the request headers and append the custom user ID identification
  if (currentUserId) {
    const clonedRequest = req.clone({
      setHeaders: {
        'X-User-Id': currentUserId // Or Authorization: `Bearer ${token}`
      }
    });
    return next(clonedRequest);
  }

  // Otherwise pass the original request through unchanged
  return next(req);
};