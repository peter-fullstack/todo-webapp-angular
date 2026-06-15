import { Routes } from '@angular/router';

import { TodoListComponent } from './features/todos/components/todo-list.component';
import { TodoFormComponent } from './features/todos/components/todo-form.component';

export const routes: Routes = [
  { path: '', redirectTo: 'todos', pathMatch: 'full' },
  { path: 'todos', component: TodoListComponent },
  { path: 'todos/add', component: TodoFormComponent },
  { path: 'todos/edit/:id', component: TodoFormComponent } // Reuse the same form component for edit!
];