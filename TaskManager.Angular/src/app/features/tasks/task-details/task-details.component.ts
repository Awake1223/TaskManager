import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TaskDto, TaskStatus } from '../../../core/models/task.model';

@Component({
  selector: 'app-task-details',
  standalone: false,
  templateUrl: './task-details.component.html',
  styleUrls: ['./task-details.component.css']
})
export class TaskDetailsComponent {
  @Input() task: TaskDto | null = null;
  @Output() closed = new EventEmitter<void>();

  readonly statusLabels: Record<TaskStatus, string> = {
    [TaskStatus.New]: 'Новая',
    [TaskStatus.InProgress]: 'В работе',
    [TaskStatus.OnHold]: 'Приостановлена',
    [TaskStatus.Done]: 'Готово',
    [TaskStatus.Cancelled]: 'Отменена'
  };

  close(): void {
    this.closed.emit();
  }
}
