import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProjectApiService } from '../../../core/services/project-api.service';
import { ProjectDto } from '../../../core/models/project.model';

@Component({
  selector: 'app-project-form',
  standalone: false,
  templateUrl: './project-form.component.html',
  styleUrls: ['./project-form.component.css']
})
export class ProjectFormComponent implements OnChanges {
  @Input() project: ProjectDto | null = null;

  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  form: FormGroup;
  saving = false;
  error: string | null = null;

  constructor(private fb: FormBuilder, private api: ProjectApiService) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(2000)]]
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['project']) {
      this.error = null;
      if (this.project) {
        this.form.patchValue({
          name: this.project.name,
          description: this.project.description ?? ''
        });
      } else {
        this.form.reset({ name: '', description: '' });
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

    const value = {
      name: this.form.value.name,
      description: this.form.value.description || null
    };

    const req$ = this.project
      ? this.api.update(this.project.id, value)
      : this.api.create(value);

    req$.subscribe({
      next: () => {
        this.saving = false;
        this.saved.emit();
      },
      error: (err) => {
        this.saving = false;
        this.error = err?.error?.error ?? 'Не удалось сохранить проект.';
      }
    });
  }

  close(): void {
    this.cancelled.emit();
  }
}
