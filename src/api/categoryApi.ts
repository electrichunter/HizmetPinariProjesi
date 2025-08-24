import apiClient from './client';
import { Category } from '@/types/api';

export const getCategories = () => apiClient.get<Category[]>('/category');

// Gelecekte eklenecek diğer API çağrıları
// export const createCategory = (data) => apiClient.post('/category', data);