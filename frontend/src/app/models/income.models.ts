export interface Income {
  id: number;
  name: string;
  periodicity: number;
  isVariable: boolean;
  expectedAmount: number | null;
  expectedReceiptDay: number;
  startMonth: string | null;
  endMonth: string | null;
}

export interface IncomeRequest {
  name: string;
  periodicity: number;
  isVariable: boolean;
  expectedAmount?: number | null;
  expectedReceiptDay: number;
  startMonth?: string | null;
  endMonth?: string | null;
}

export interface IncomeReceiptRequest {
  amount: number;
  receiptDate: string;
  notes?: string | null;
}