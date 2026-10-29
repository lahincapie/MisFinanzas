using MisFinanzas.Domain.Categories;

namespace MisFinanzas.Application.Categories
{
    /// <summary>
    /// Categorías que se crean automáticamente para cada usuario nuevo al registrarse (RF-USR-01).
    /// Son un punto de partida genérico; el usuario puede editarlas, desactivarlas o crear más.
    /// </summary>
    public static class DefaultCategories
    {
        // Nombre, ícono (Material Symbols) y color (hex de la paleta de la app).
        private static readonly (string Name, string Icon, string Color)[] Seed =
        {
            ("Vivienda",     "home",            "#7FB3D5"),
            ("Servicios",    "receipt_long",    "#48C9B0"),
            ("Alimentación", "restaurant",      "#82C596"),
            ("Transporte",   "directions_car",  "#E59866"),
            ("Salud",        "local_hospital",  "#EC7063"),
            ("Educación",    "school",          "#AF7AC5"),
            ("Compras",      "shopping_cart",   "#F4D03F"),
            ("Créditos",     "credit_card",     "#5B4FC4"),
            ("Ahorro",       "savings",         "#48C9B0"),
            ("Otros",        "category",        "#8A8A9A"),
        };

        /// <summary>Construye la lista de categorías por defecto para un usuario.</summary>
        public static List<Category> BuildFor(string userId)
        {
            var now = DateTime.UtcNow;
            var categories = new List<Category>();

            for (var i = 0; i < Seed.Length; i++)
            {
                var (name, icon, color) = Seed[i];
                categories.Add(new Category
                {
                    Name = name,
                    Icon = icon,
                    Color = color,
                    Order = i + 1,
                    UserId = userId,
                    IsActive = true,
                    CreatedAt = now
                });
            }

            return categories;
        }
    }
}
