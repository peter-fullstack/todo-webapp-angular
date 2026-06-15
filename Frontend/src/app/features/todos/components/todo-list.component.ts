// features/todos/todo-list.component.ts
import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TodoService } from '../services/todo.service';
import { TodoItemComponent } from './todo-item.component';
import { UserService } from '../../../core/services/user.service';

// Simple interface matching your backend DTO structure
export interface TodoResponseDto {
  id: number;
  title: string;
  description: string;
  isCompleted: boolean;
}

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [CommonModule, RouterModule, TodoItemComponent],
  templateUrl: './todo-list.component.html'
})
export class TodoListComponent implements OnInit {
  // Using Angular Signals for efficient, modern reactivity
  todos = signal<TodoResponseDto[]>([]);
  isLoading = signal<boolean>(true);

  userService = inject(UserService);
  
  constructor(private todoService: TodoService) {}

  ngOnInit(): void {
    if (this.userService.currentUser()) {
      this.loadTodos();
    }
  }

  loadTodos(): void {
    this.isLoading.set(true);
    this.todoService.getAll().subscribe(todos => {
      this.todos.set(todos);
      this.isLoading.set(false);
    });
  }
}