export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface Person {
  id: string;
  name: string;
  age: number;
  transactions?: Transaction[];
}

export interface Transaction {
  id: string;
  description: string;
  value: number;
  type: 'Income' | 'Expense';
  personId: string;
  personName?: string;
}

export interface PersonSummary {
  personId: string;
  personName: string;
  totalIncome: number;
  totalExpenses: number;
  balance: number;
}

export interface SummaryResponse {
  totalIncome: number;
  totalExpenses: number;
  netBalance: number;
  personSummaries: PersonSummary[];
}
