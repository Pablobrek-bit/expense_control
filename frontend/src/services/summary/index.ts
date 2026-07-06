import api from '../api';
import type { SummaryResponse } from '../../types';

export const summaryService = {
  getSummary: async () => {
    const response = await api.get<SummaryResponse>('/summary');
    return response.data;
  }
};
