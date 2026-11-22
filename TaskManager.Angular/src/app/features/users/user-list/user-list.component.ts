import { Component, OnInit } from '@angular/core';
import { UserApiService } from '../../../core/services/user-api.service';
import { UserDto } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-list',
  standalone: false,
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {
  users: UserDto[] = [];
  page = 1;
  pageSize = 20;
  totalCount = 0;
  search = '';

  loading = false;
  error: string | null = null;

  showForm = false;

  constructor(private api: UserApiService) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = null;
    this.api.getPaged(this.page, this.pageSize, this.search).subscribe({
      next: (res) => {
        this.users = res.items;
        this.totalCount = res.totalCount;
        this.page = res.page;
        this.pageSize = res.pageSize;
        this.loading = false;
      },
      error: (err) => {
        this.error = err?.error?.error ?? 'Не удалось загрузить пользователей.';
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
    this.showForm = true;
  }

  onFormSaved(): void {
    this.showForm = false;
    this.load();
  }

  onFormCancelled(): void {
    this.showForm = false;
  }

  delete(user: UserDto): void {
    if (!confirm(`Удалить (soft) пользователя "${user.firstName} ${user.lastName}"?`)) return;
    this.api.softDelete(user.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error = err?.error?.error ?? 'Не удалось удалить пользователя.'
    });
  }
}
