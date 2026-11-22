export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  isDeleted: boolean;
  createdAt: string;
}

export interface CreateUserRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}
