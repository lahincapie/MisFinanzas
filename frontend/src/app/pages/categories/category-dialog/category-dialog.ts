import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Category } from '../../../models/category.models';

@Component({
  selector: 'app-category-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './category-dialog.html',
  styleUrl: './category-dialog.css'
})
export class CategoryDialog {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<CategoryDialog>);
  data = inject<Category | null>(MAT_DIALOG_DATA);

  readonly colors = [
    { name: 'Azul', value: '#7FB3D5' },
    { name: 'Verde', value: '#82C596' },
    { name: 'Morado', value: '#5B4FC4' },
    { name: 'Naranja', value: '#E59866' },
    { name: 'Rojo', value: '#EC7063' },
    { name: 'Amarillo', value: '#F4D03F' },
    { name: 'Violeta', value: '#AF7AC5' },
    { name: 'Turquesa', value: '#48C9B0' }
  ];

  readonly icons = [
    { name: 'Hogar', value: 'home' },
    { name: 'Educación', value: 'school' },
    { name: 'Comida', value: 'restaurant' },
    { name: 'Transporte', value: 'directions_car' },
    { name: 'Salud', value: 'local_hospital' },
    { name: 'Compras', value: 'shopping_cart' },
    { name: 'Servicios', value: 'receipt_long' },
    { name: 'Otros', value: 'category' }
  ];

  form = this.fb.nonNullable.group({
    name: ['', [Validators.required]],
    description: [''],
    color: ['#7FB3D5'],
    icon: ['home'],
    order: [1, [Validators.required, Validators.min(1), Validators.max(50)]]
  });

  isEdit = this.data !== null;

  constructor() {
    if (this.data) {
      this.form.setValue({
        name: this.data.name,
        description: this.data.description ?? '',
        color: this.data.color ?? '#7FB3D5',
        icon: this.data.icon ?? 'home',
        order: this.data.order
      });
    }
  }

  selectColor(color: string): void {
    this.form.controls.color.setValue(color);
  }

  selectIcon(icon: string): void {
    this.form.controls.icon.setValue(icon);
  }

  save(): void {
    if (this.form.invalid) return;
    this.dialogRef.close(this.form.getRawValue());
  }

  cancel(): void {
    this.dialogRef.close();
  }
}