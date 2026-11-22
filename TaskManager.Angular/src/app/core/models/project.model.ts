export interface ProjectDto {
  id: string;
  name: string;
  description: string | null;
  createdAt: string;
  createdByUserId: string;
  createdByUserName: string;
  tasksCount: number;
}

export interface CreateProjectRequest {
  name: string;
  description: string | null;
}

export interface UpdateProjectRequest {
  name: string;
  description: string | null;
}
