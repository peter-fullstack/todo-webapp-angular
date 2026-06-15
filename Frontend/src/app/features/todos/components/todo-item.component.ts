import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Todo } from '../models/todo.models';
import { RouterModule } from '@angular/router';
import { TodoService } from '../services/todo.service';

@Component({
  selector: 'app-todo-item',
  standalone: true,
  imports: [FormsModule, FormsModule, RouterModule],
  templateUrl: './todo-item.component.html',
  styles: [`
    .todo-item-row { display: flex; gap: 10px; align-items: center; margin-bottom: 8px; }
    .completed .todo-title { text-decoration: line-through; color: gray; }
    .btn-delete { background-color: #ff4d4d; color: white; border: none; padding: 4px 8px; cursor: pointer; }
  `]
})
export class TodoItemComponent {
  todoService = inject(TodoService);
   
  todo = input.required<Todo>();
  
  delete = output<number>();
  update = output<{ id: number; title: string; isCompleted: boolean }>();
 
  onToggleCompleted() {
    this.todoService.update({
      id: this.todo().id,
      title: this.todo().title,
      description: this.todo().description,
      isCompleted: !this.todo().isCompleted   
    }).subscribe(updatedTodo => {
      this.update.emit({
        id: updatedTodo.id,
        title: updatedTodo.title,
        isCompleted: updatedTodo.isCompleted
      });
    });
  }
}
