import api from '../api';
import type { Transaction, PaginatedResponse } from '../../types';

export const transactionService = {
  getAll: async (page = 1, pageSize = 10, type?: string, personId?: string) => {
    const params = new URLSearchParams({ page: page.toString(), pageSize: pageSize.toString() });
    if (type) params.append('type', type);
    if (personId) params.append('personId', personId);
    
    const response = await api.get<PaginatedResponse<Transaction>>(`/transactions?${params.toString()}`);
    return response.data;
  },
  
  create: async (data: { description: string; value: number; type: 'Income' | 'Expense'; personId: string }) => {
    const response = await api.post<Transaction>('/transactions', data);
    return response.data;
  },

  update: async (id: string, data: { description: string; value: number; type: 'Income' | 'Expense' }) => {
    const response = await api.put<Transaction>(`/transactions/${id}`, data);
    return response.data;
  },

  delete: async (id: string) => {
    await api.delete(`/transactions/${id}`);
  }
};
