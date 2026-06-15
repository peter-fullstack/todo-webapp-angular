import { Component, signal } from '@angular/core';
import { TodoDashboardComponent } from './features/todos/todo-dashboard.component';

@Component({
  selector: 'app-root',
  imports: [TodoDashboardComponent],
  template: `<app-todo-dashboard />`,
  standalone: true,
})
export class App {
  protected readonly title = signal('todo-app');
}
