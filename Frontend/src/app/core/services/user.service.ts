// core/services/user.service.ts
import { Injectable, signal, computed } from '@angular/core';

@Injectable({
  providedIn: 'root' // 👈 Essential: Makes this a singleton shared across the whole app
})
export class UserService {
  // 1. Keep the writable signal private so components can't mutate it directly
  private currentUserState = signal<string | null>(null);

  // 2. Expose a read-only signal for the application to consume
  currentUser = this.currentUserState.asReadonly();

  // 3. Simple action setter method to update the state
  setSelectedUser(userId: string): void {
    this.currentUserState.set(userId);
  }

  clearUser(): void {
    this.currentUserState.set(null);
  }
}