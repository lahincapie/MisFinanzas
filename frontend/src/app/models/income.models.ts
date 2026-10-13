export interface Income {
  id: number;
  name: string;
  periodicity: number;
  isVariable: boolean;
  expectedAmount: number | null;
  expectedReceiptDay: number;
  startMonth: string | null;
  endMonth: string | null;
  anchorMonth: string | null;
}

export interface IncomeRequest {
  name: string;
  periodicity: number;
  isVariable: boolean;
  expectedAmount?: number | null;
  expectedReceiptDay: number;
  startMonth?: string | null;
  endMonth?: string | null;
  anchorMonth?: string | null;
}

export interface IncomeReceiptRequest {
  amount: number;
  receiptDate: string;
  notes?: string | null;
}
export interface OneOffIncomeRequest {
  name: string;
  amount: number;
  notes?: string | null;
}