export interface Category {
  id: number;
  name: string;
  description: string;
  color: string;
  icon: string;
  order: number;
}

export interface CategoryRequest {
  name: string;
  description?: string;
  color?: string;
  icon?: string;
  order: number;
}