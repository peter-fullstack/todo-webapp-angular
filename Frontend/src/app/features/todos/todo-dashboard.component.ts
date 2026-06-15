import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TodoService } from './services/todo.service';
import { TodoItemComponent } from './components/todo-item.component';
import { Todo } from './models/todo.models';
import { RouterModule } from '@angular/router';
import { TodoResponseDto } from './components/todo-list.component';
import { UserService } from '../../core/services/user.service';

import { UserDropdownComponent } from '../users/components/user-dropdown.component';

@Component({
  selector: 'app-todo-dashboard',
  standalone: true,
  imports: [FormsModule, RouterModule, UserDropdownComponent],
  templateUrl: './todo-dashboard.component.html',
})

export class TodoDashboardComponent implements OnInit {
  private userService = inject(UserService);

  ngOnInit() {
  }
}
