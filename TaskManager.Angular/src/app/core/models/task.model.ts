export enum TaskStatus {
  New = 'New',
  InProgress = 'InProgress',
  OnHold = 'OnHold',
  Done = 'Done',
  Cancelled = 'Cancelled'
}

export interface TaskDto {
  id: string;
  name: string;
  description: string | null;
  status: TaskStatus;
  projectId: string;
  projectName: string;
  assigneeId: string | null;
  assigneeName: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateTaskRequest {
  name: string;
  description: string | null;
  projectId: string;
  assigneeId: string | null;
}

export interface UpdateTaskRequest {
  name: string;
  description: string | null;
  assigneeId: string | null;
}

export interface UpdateTaskStatusRequest {
  status: TaskStatus;
}
