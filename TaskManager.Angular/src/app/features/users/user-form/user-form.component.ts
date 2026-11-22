import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserApiService } from '../../../core/services/user-api.service';

@Component({
  selector: 'app-user-form',
  standalone: false,
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})
export class UserFormComponent {
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  form: FormGroup;
  saving = false;
  error: string | null = null;

  constructor(private fb: FormBuilder, private api: UserApiService) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.error = null;

    this.api.create(this.form.value).subscribe({
      next: () => { this.saving = false; this.saved.emit(); },
      error: (err) => { this.saving = false; this.error = err?.error?.error ?? 'Не удалось создать пользователя.'; }
    });
  }

  close(): void {
    this.cancelled.emit();
  }
}
