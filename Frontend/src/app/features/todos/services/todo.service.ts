import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs'; // 👈 Import Observable
import { Todo, CreateTodoDto, UpdateTodoDto } from '../models/todo.models';
import { UserService } from '../../../core/services/user.service';

@Injectable({ providedIn: 'root' })
export class TodoService {
  private userService = inject(UserService);

  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5200/api/todos';

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'X-User-Id': this.userService.currentUser()?.toString() || ''
    });
  }

  // Return Observable directly instead of a Promise
  getAll(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.apiUrl, { headers: this.getHeaders() });

    //return [] as unknown as Observable<Todo[]>; // Placeholder to satisfy return type
  }

  getById(id: number): Observable<Todo> {
    return this.http.get<Todo>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  create(dto: CreateTodoDto): Observable<Todo> {
    return this.http.post<Todo>(this.apiUrl, dto, { headers: this.getHeaders() });
  }

  update(dto: UpdateTodoDto): Observable<Todo> {
    return this.http.put<Todo>(`${this.apiUrl}`, dto, { headers: this.getHeaders() });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}