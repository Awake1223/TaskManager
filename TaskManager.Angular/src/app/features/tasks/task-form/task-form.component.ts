import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TaskApiService } from '../../../core/services/task-api.service';
import { ProjectApiService } from '../../../core/services/project-api.service';
import { UserApiService } from '../../../core/services/user-api.service';
import { ProjectDto } from '../../../core/models/project.model';
import { UserDto } from '../../../core/models/user.model';
import { TaskDto } from '../../../core/models/task.model';

@Component({
  selector: 'app-task-form',
  standalone: false,
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.css']
})
export class TaskFormComponent implements OnInit, OnChanges {
  @Input() task: TaskDto | null = null;

  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  form: FormGroup;
  saving = false;
  error: string | null = null;

  projects: ProjectDto[] = [];
  users: UserDto[] = [];

  constructor(
    private fb: FormBuilder,
    private tasksApi: TaskApiService,
    private projectsApi: ProjectApiService,
    private usersApi: UserApiService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(2000)]],
      projectId: ['', [Validators.required]],
      assigneeId: [null as string | null]
    });
  }

  ngOnInit(): void {
    this.projectsApi.getPaged(1, 100, '').subscribe(res => this.projects = res.items);
    this.usersApi.getPaged(1, 100, '').subscribe(res => this.users = res.items);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['task']) {
      this.error = null;
      if (this.task) {
        this.form.patchValue({
          name: this.task.name,
          description: this.task.description ?? '',
          projectId: this.task.projectId,
          assigneeId: this.task.assigneeId
        });
        // projectId блокируем при редактировании (по ТЗ проект у задачи не меняется)
        this.form.get('projectId')?.disable();
      } else {
        this.form.reset({ name: '', description: '', projectId: '', assigneeId: null });
        this.form.get('projectId')?.enable();
      }
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.error = null;

    if (this.task) {
      const value = {
        name: this.form.value.name,
        description: this.form.value.description || null,
        assigneeId: this.form.value.assigneeId
      };
      this.tasksApi.update(this.task.id, value).subscribe({
        next: () => { this.saving = false; this.saved.emit(); },
        error: (err) => { this.saving = false; this.error = err?.error?.error ?? 'Не удалось сохранить задачу.'; }
      });
    } else {
      const value = {
        name: this.form.value.name,
        description: this.form.value.description || null,
        projectId: this.form.value.projectId,
        assigneeId: this.form.value.assigneeId
      };
      this.tasksApi.create(value).subscribe({
        next: () => { this.saving = false; this.saved.emit(); },
        error: (err) => { this.saving = false; this.error = err?.error?.error ?? 'Не удалось создать задачу.'; }
      });
    }
  }

  close(): void {
    this.cancelled.emit();
  }
}
