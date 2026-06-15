
import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../core/services/user.service';

interface UserOption {
  id: string;
  name: string;
}

@Component({
  selector: 'app-user-dropdown',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-dropdown.component.html'
})
export class UserDropdownComponent implements OnInit {
  private userService = inject(UserService);

  // Mock users list matching your requirement
  users = signal<UserOption[]>([
    { id: '1', name: 'Alice Smith' },
    { id: '2', name: 'Bob Jones' },
    { id: '3', name: 'Charlie Brown' }
  ]);

  // Bind this locally to reflect the current active service state
  selectedUserId: string = '';

  ngOnInit(): void {
    // Sync dropdown value with the global signal if a user is already set
    const current = this.userService.currentUser();
    if (current) {
      this.selectedUserId = current;
    }
  }

  onUserChange(newUserId: string): void {
    if (newUserId) {
      this.userService.setSelectedUser(newUserId);
    } else {
      this.userService.clearUser();
    }
  }
}