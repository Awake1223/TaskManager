import { Component, OnInit } from '@angular/core';
import { TaskApiService } from '../../../core/services/task-api.service';
import { TaskDto, TaskStatus } from '../../../core/models/task.model';

@Component({
  selector: 'app-task-list',
  standalone: false,
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.css']
})
export class TaskListComponent implements OnInit {
  tasks: TaskDto[] = [];
  page = 1;
  pageSize = 20;
  totalCount = 0;
  search = '';

  loading = false;
  error: string | null = null;

  showForm = false;
  editingTask: TaskDto | null = null;

  showDetails = false;
  detailsTask: TaskDto | null = null;

  readonly statuses: TaskStatus[] = [
    TaskStatus.New,
    TaskStatus.InProgress,
    TaskStatus.OnHold,
    TaskStatus.Done,
    TaskStatus.Cancelled
  ];

  readonly statusLabels: Record<TaskStatus, string> = {
    [TaskStatus.New]: 'Новая',
    [TaskStatus.InProgress]: 'В работе',
    [TaskStatus.OnHold]: 'Приостановлена',
    [TaskStatus.Done]: 'Готово',
    [TaskStatus.Cancelled]: 'Отменена'
  };

  constructor(private api: TaskApiService) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = null;
    this.api.getPaged(this.page, this.pageSize, this.search).subscribe({
      next: (res) => {
        this.tasks = res.items;
        this.totalCount = res.totalCount;
        this.page = res.page;
        this.pageSize = res.pageSize;
        this.loading = false;
      },
      error: (err) => {
        this.error = err?.error?.error ?? 'Не удалось загрузить задачи.';
        this.loading = false;
      }
    });
  }

  onSearch(value: string): void {
    this.search = value;
    this.page = 1;
    this.load();
  }

  onPageChange(p: number): void {
    this.page = p;
    this.load();
  }

  openCreate(): void {
    this.editingTask = null;
    this.showForm = true;
  }

  openEdit(task: TaskDto): void {
    this.editingTask = task;
    this.showForm = true;
  }

  openDetails(task: TaskDto): void {
    this.detailsTask = task;
    this.showDetails = true;
  }

  onFormSaved(): void {
    this.showForm = false;
    this.editingTask = null;
    this.load();
  }

  onFormCancelled(): void {
    this.showForm = false;
    this.editingTask = null;
  }

  onDetailsClosed(): void {
    this.showDetails = false;
    this.detailsTask = null;
  }

  changeStatus(task: TaskDto, status: TaskStatus): void {
    this.api.updateStatus(task.id, { status }).subscribe({
      next: (updated) => {
        task.status = updated.status;
        task.updatedAt = updated.updatedAt;
      },
      error: (err) => this.error = err?.error?.error ?? 'Не удалось изменить статус.'
    });
  }

  delete(task: TaskDto): void {
    if (!confirm(`Удалить задачу "${task.name}"?`)) return;
    this.api.delete(task.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error = err?.error?.error ?? 'Не удалось удалить задачу.'
    });
  }
}
