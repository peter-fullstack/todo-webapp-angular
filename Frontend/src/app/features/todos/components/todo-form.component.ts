// features/todos/todo-form.component.ts
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TodoService } from '../services/todo.service';

@Component({
  selector: 'app-todo-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './todo-form.component.html'
})
export class TodoFormComponent implements OnInit {
  todoForm!: FormGroup;
  isEditMode = false;
  todoId?: number;

  constructor(
    private fb: FormBuilder,
    private todoService: TodoService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForm();

    // Check if an ID exists in the route params to enable Edit Mode
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.isEditMode = true;
        this.todoId = +id;
        this.loadTodoDetails(this.todoId);
      }
    });
  }

  private initForm(): void {
    this.todoForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]]
    });
  }

  private loadTodoDetails(id: number): void {
    this.todoService.getById(id).subscribe(todo => {
      this.todoForm.patchValue({
        title: todo.title,
        description: todo.description
      });
    });
  }

  async onSubmit(): Promise<void> {
    if (this.todoForm.invalid) return;

    const todoData = this.todoForm.value;

    if (this.isEditMode && this.todoId) {
      this.todoService.update({ id: this.todoId, ...todoData }).subscribe(() => {
        this.router.navigate(['/todos']);
      });
    } else {
      this.todoService.create(todoData).subscribe(() => {
        this.router.navigate(['/todos']);
      });
    }
  }
}