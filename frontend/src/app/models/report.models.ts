export interface CategorySpend {
  categoryName: string;
  total: number;
}

export interface ExpenseSpend {
  expenseId: number;
  expenseName: string;
  categoryName: string;
  total: number;
}