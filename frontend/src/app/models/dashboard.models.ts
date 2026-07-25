export interface DashboardExpenseItem {
  expenseId: number;
  name: string;
  categoryName: string;
  status: string;
  paidAmount: number | null;
  projectedAmount: number;
  dueDay: number;
  isOverdue: boolean;
}

export interface DashboardIncomeItem {
  incomeId: number;
  name: string;
  status: string;
  receivedAmount: number | null;
  projectedAmount: number;
  expectedReceiptDay: number;
  isLate: boolean;
}

export interface DashboardData {
  month: string;
  realIncome: number;
  realExpense: number;
  realBalance: number;
  projectedIncome: number;
  projectedExpense: number;
  projectedBalance: number;
  expenses: DashboardExpenseItem[];
  incomes: DashboardIncomeItem[];
}