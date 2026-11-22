import { Component, OnInit } from '@angular/core';
import { ProjectApiService } from '../../../core/services/project-api.service';
import { ProjectDto } from '../../../core/models/project.model';

@Component({
  selector: 'app-project-list',
  standalone: false,
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.css']
})
export class ProjectListComponent implements OnInit {
  projects: ProjectDto[] = [];
  page = 1;
  pageSize = 20;
  totalCount = 0;
  search = '';

  loading = false;
  error: string | null = null;

  // modal state
  showForm = false;
  editingProject: ProjectDto | null = null;

  constructor(private api: ProjectApiService) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = null;
    this.api.getPaged(this.page, this.pageSize, this.search).subscribe({
      next: (res) => {
        this.projects = res.items;
        this.totalCount = res.totalCount;
        this.page = res.page;
        this.pageSize = res.pageSize;
        this.loading = false;
      },
      error: (err) => {
        this.error = err?.error?.error ?? 'Не удалось загрузить проекты.';
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
    this.editingProject = null;
    this.showForm = true;
  }

  openEdit(project: ProjectDto): void {
    this.editingProject = project;
    this.showForm = true;
  }

  onFormSaved(): void {
    this.showForm = false;
    this.editingProject = null;
    this.load();
  }

  onFormCancelled(): void {
    this.showForm = false;
    this.editingProject = null;
  }

  delete(project: ProjectDto): void {
    if (!confirm(`Удалить проект "${project.name}"?`)) return;
    this.api.delete(project.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error = err?.error?.error ?? 'Не удалось удалить проект.'
    });
  }
}
