import type { PaginatedResponse, Person } from '../../types';
import api from '../api';

export const personService = {
  getAll: async (page = 1, pageSize = 10, name?: string, age?: number) => {
    const params = new URLSearchParams({ page: page.toString(), pageSize: pageSize.toString() });
    if (name) params.append('name', name);
    if (age) params.append('age', age.toString());
    
    const response = await api.get<PaginatedResponse<Person>>(`/persons?${params.toString()}`);
    return response.data;
  },
  
  getById: async (id: string) => {
    const response = await api.get<Person>(`/persons/${id}`);
    return response.data;
  },
  
  create: async (data: { name: string; age: number }) => {
    const response = await api.post<Person>('/persons', data);
    return response.data;
  },

  update: async (id: string, data: { name: string; age: number }) => {
    const response = await api.put<Person>(`/persons/${id}`, data);
    return response.data;
  },

  delete: async (id: string) => {
    await api.delete(`/persons/${id}`);
  }
};
