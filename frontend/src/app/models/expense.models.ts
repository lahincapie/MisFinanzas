export interface Expense {
  id: number;
  name: string;
  categoryId: number;
  categoryName: string;
  periodicity: number;
  isVariable: boolean;
  expectedAmount: number | null;
  cutoffDay: number;
  dueDay: number;
  suspensionDay: number;
  startMonth: string | null;
  endMonth: string | null;
  anchorMonth: string | null;
  reference: string | null;
  contract: string | null;
}

export interface ExpenseRequest {
  name: string;
  categoryId: number;
  periodicity: number;
  isVariable: boolean;
  expectedAmount?: number | null;
  cutoffDay: number;
  dueDay: number;
  suspensionDay: number;
  startMonth?: string | null;
  endMonth?: string | null;
  anchorMonth?: string | null;
  reference?: string | null;
  contract?: string | null;
}

export interface ExpensePaymentRequest {
  amount: number;
  paymentDate: string;
  paymentMethodId: number;
  notes?: string | null;
}