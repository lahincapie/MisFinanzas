import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CategoryService } from '../../services/category.service';
import { Category } from '../../models/category.models';

@Component({
  selector: 'app-categories',
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './categories.html',
  styleUrl: './categories.css'
})
export class Categories implements OnInit {
  private categoryService = inject(CategoryService);
  private fb = inject(FormBuilder);

  categories = signal<Category[]>([]);
  editingId = signal<number | null>(null);
  errorMessage = signal('');
  displayedColumns = ['name', 'description', 'order', 'actions'];

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

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => this.categories.set(
        [...data].sort((a, b) => a.name.localeCompare(b.name))
      ),
      error: (err) => console.error('Error al cargar categorías:', err)
    });
  }

  startEdit(cat: Category): void {
    this.editingId.set(cat.id);
    this.form.setValue({
      name: cat.name,
      description: cat.description ?? '',
      color: cat.color ?? '#7FB3D5',
      icon: cat.icon ?? 'home',
      order: cat.order
    });
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.form.reset();
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    const id = this.editingId();
    const request = id === null
      ? this.categoryService.create(this.form.getRawValue())
      : this.categoryService.update(id, this.form.getRawValue());

    request.subscribe({
      next: () => {
        this.cancelEdit();
        this.loadCategories();
      },
      error: (err) => console.error('Error al guardar:', err)
    });
  }

  deleteCategory(cat: Category): void {
    if (!confirm(`¿Eliminar la categoría "${cat.name}"?`)) return;

    this.errorMessage.set('');
    this.categoryService.delete(cat.id).subscribe({
      next: () => this.loadCategories(),
      error: (err) => {
        if (err.status === 409) {
          this.errorMessage.set('No se puede eliminar: la categoría está en uso por gastos.');
        } else {
          this.errorMessage.set('No se pudo eliminar la categoría.');
        }
      }
    });
  }
}