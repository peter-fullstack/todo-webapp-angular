export interface Todo {
  id: number;
  title: string;
  description: string;
  isCompleted: boolean;
}

export interface CreateTodoDto {
  title: string;
  description: string;
}

export interface UpdateTodoDto {
  id: number;
  title: string;
  description: string;
  isCompleted: boolean;
}

export interface User {
  id: number;
  name: string;
}
