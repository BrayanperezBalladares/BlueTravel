using BlueTravel.Models;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // ? NUEVO: Solo cargar datos si la BD está vacía
            // Si ya existen hospedajes, tours, etc., NO eliminar datos
            if (await context.Hospedajes.AnyAsync() || 
                await context.Tours.AnyAsync() || 
                await context.Restaurantes.AnyAsync() ||
                await context.Ofertas.AnyAsync())
            {
                // La BD ya tiene datos, así que NO hacer nada
                // Esto permite que los datos agregados por el usuario persistan
                return;
            }

            // Solo ejecutar limpieza si es la primera vez (BD vacía)
            try
            {
                // Eliminar en orden correcto (respetando relaciones FK)
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Reservas");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Pagos");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Hospedajes");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Tours");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Ofertas");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Restaurantes");
                await context.Database.ExecuteSqlRawAsync("DELETE FROM Transportes");
            }
            catch { }

            // Hospedajes con nuevos campos
            var hospedajes = new List<Hospedaje>
            {
                new Hospedaje { Nombre = "Hotel Papagayo Peninsula", Ubicacion = "Papagayo, Guanacaste", Descripcion = "Resort de lujo con vista al océano Pacífico. Incluye piscinas infinitas, spa y restaurante gourmet.", PrecioPorNoche = 120.00m, ImagenUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800", CapacidadMaxima = 4, PersonasIncluidasEnPrecio = 2, CargoPorPersonaExtra = 25.00m, TipoHospedaje = "Resort", PermiteNinos = true, PermiteMascotas = false, HoraCheckIn = 15, HoraCheckOut = 11 },
                new Hospedaje { Nombre = "Beach Resort Tamarindo", Ubicacion = "Tamarindo, Guanacaste", Descripcion = "Ubicado frente a la playa perfecta para surf. Habitaciones con balcón y vista al mar.", PrecioPorNoche = 85.00m, ImagenUrl = "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=800", CapacidadMaxima = 3, PersonasIncluidasEnPrecio = 2, CargoPorPersonaExtra = 20.00m, TipoHospedaje = "Hotel", PermiteNinos = true, PermiteMascotas = true, HoraCheckIn = 14, HoraCheckOut = 12 },
                new Hospedaje { Nombre = "Eco Lodge Rincón de la Vieja", Ubicacion = "Rincón de la Vieja, Guanacaste", Descripcion = "Hospedaje ecológico rodeado de naturaleza. Acceso a cataratas y aguas termales.", PrecioPorNoche = 95.00m, ImagenUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800", CapacidadMaxima = 2, PersonasIncluidasEnPrecio = 2, CargoPorPersonaExtra = 15.00m, TipoHospedaje = "Cabaña", PermiteNinos = true, PermiteMascotas = true, HoraCheckIn = 16, HoraCheckOut = 10 },
                new Hospedaje { Nombre = "Flamingo Beach Hotel", Ubicacion = "Flamingo, Guanacaste", Descripcion = "Hotel boutique frente a playa Flamingo. Perfecto para familias y parejas.", PrecioPorNoche = 110.00m, ImagenUrl = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800", CapacidadMaxima = 5, PersonasIncluidasEnPrecio = 2, CargoPorPersonaExtra = 18.00m, TipoHospedaje = "Hotel", PermiteNinos = true, PermiteMascotas = false, HoraCheckIn = 15, HoraCheckOut = 11 },
                new Hospedaje { Nombre = "Coco Beach Villas", Ubicacion = "Playas del Coco, Guanacaste", Descripcion = "Villas privadas con cocina equipada. Ideal para estancias largas.", PrecioPorNoche = 75.00m, ImagenUrl = "https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=800", CapacidadMaxima = 6, PersonasIncluidasEnPrecio = 3, CargoPorPersonaExtra = 12.00m, TipoHospedaje = "Villa", PermiteNinos = true, PermiteMascotas = true, HoraCheckIn = 14, HoraCheckOut = 12 },
                new Hospedaje { Nombre = "Samara Treehouse Resort", Ubicacion = "Sámara, Guanacaste", Descripcion = "Cabañas en los árboles con diseño único. Experiencia de selva y playa.", PrecioPorNoche = 130.00m, ImagenUrl = "https://images.unsplash.com/photo-1564501049412-61c2a3083791?w=800", CapacidadMaxima = 2, PersonasIncluidasEnPrecio = 2, CargoPorPersonaExtra = 30.00m, TipoHospedaje = "Cabaña", PermiteNinos = false, PermiteMascotas = false, HoraCheckIn = 16, HoraCheckOut = 10 }
            };

            // Tours con nuevos campos
            var tours = new List<Tour>
            {
                new Tour { Nombre = "Aventura en Tirolesa - Monteverde", Descripcion = "Sobrevuela el bosque nuboso en 12 cables de tirolesa.", Ubicacion = "Monteverde, Guanacaste", Precio = 65.00m, Duracion = 1, FechaDisponible = DateTime.Now.AddDays(7), ImagenUrl = "https://images.unsplash.com/photo-1527004013197-933c4bb611b3?w=800", CupoMaximo = 20, CuposReservados = 0, PrecioNino = 45.00m, PrecioSenior = 55.00m, DescuentoGrupo = 10, NivelDificultad = "Moderado", EdadMinima = 5, EdadMaxima = 75, RequiereConfirmacion = false },
                new Tour { Nombre = "Tour de Snorkel en Islas Catalinas", Descripcion = "Explora la vida marina de las Islas Catalinas. Incluye equipo y almuerzo.", Ubicacion = "Flamingo, Guanacaste", Precio = 85.00m, Duracion = 1, FechaDisponible = DateTime.Now.AddDays(5), ImagenUrl = "https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=800", CupoMaximo = 15, CuposReservados = 0, PrecioNino = 60.00m, PrecioSenior = 75.00m, DescuentoGrupo = 15, NivelDificultad = "Fácil", EdadMinima = 3, EdadMaxima = null, RequiereConfirmacion = false },
                new Tour { Nombre = "Rafting en Río Tenorio", Descripcion = "Aventura de rafting clase III-IV. Ideal para amantes de la adrenalina.", Ubicacion = "Tenorio, Guanacaste", Precio = 75.00m, Duracion = 1, FechaDisponible = DateTime.Now.AddDays(10), ImagenUrl = "https://images.unsplash.com/photo-1501555088652-021faa106b9b?w=800", CupoMaximo = 12, CuposReservados = 0, PrecioNino = null, PrecioSenior = 65.00m, DescuentoGrupo = 12, NivelDificultad = "Difícil", EdadMinima = 12, EdadMaxima = 70, RequiereConfirmacion = true },
                new Tour { Nombre = "Tour a Cataratas Llanos de Cortés", Descripcion = "Visita a una de las cataratas más hermosas de Costa Rica.", Ubicacion = "Bagaces, Guanacaste", Precio = 45.00m, Duracion = 1, FechaDisponible = DateTime.Now.AddDays(3), ImagenUrl = "https://images.unsplash.com/photo-1432405972618-c60b0225b8f9?w=800", CupoMaximo = 25, CuposReservados = 0, PrecioNino = 30.00m, PrecioSenior = 40.00m, DescuentoGrupo = 20, NivelDificultad = "Fácil", EdadMinima = 0, EdadMaxima = null, RequiereConfirmacion = false },
                new Tour { Nombre = "Pesca Deportiva en Alta Mar", Descripcion = "Jornada completa de pesca deportiva. Todo el equipo incluido.", Ubicacion = "Tamarindo, Guanacaste", Precio = 180.00m, Duracion = 1, FechaDisponible = DateTime.Now.AddDays(14), ImagenUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=800", CupoMaximo = 6, CuposReservados = 0, PrecioNino = null, PrecioSenior = 170.00m, DescuentoGrupo = 5, NivelDificultad = "Moderado", EdadMinima = 10, EdadMaxima = null, RequiereConfirmacion = true },
                new Tour { Nombre = "Safari en Palo Verde", Descripcion = "Recorrido en bote por el humedal. Observación de cocodrilos, aves y monos.", Ubicacion = "Palo Verde, Guanacaste", Precio = 55.00m, Duracion = 1, FechaDisponible = DateTime.Now.AddDays(6), ImagenUrl = "https://images.unsplash.com/photo-1549366021-9f761d450615?w=800", CupoMaximo = 20, CuposReservados = 0, PrecioNino = 40.00m, PrecioSenior = 50.00m, DescuentoGrupo = 15, NivelDificultad = "Fácil", EdadMinima = 0, EdadMaxima = null, RequiereConfirmacion = false }
            };

            var restaurantes = new List<Restaurante>
            {
                new Restaurante { Nombre = "Marisquería El Pulpo", TipoComida = "Mariscos", Ubicacion = "Playas del Coco, Guanacaste", Especialidad = "Ceviche de Corvina y Langosta al Ajillo", PrecioPromedio = 25.00m, ImagenUrl = "https://images.unsplash.com/photo-1559339352-11d035aa65de?w=800" },
                new Restaurante { Nombre = "Asadero La Fogata", TipoComida = "Carnes a la Parrilla", Ubicacion = "Liberia, Guanacaste", Especialidad = "Churrasco Guanacasteco", PrecioPromedio = 18.00m, ImagenUrl = "https://images.unsplash.com/photo-1544025162-d76694265947?w=800" },
                new Restaurante { Nombre = "Soda Típica Tica Linda", TipoComida = "Comida Típica Costarricense", Ubicacion = "Nicoya, Guanacaste", Especialidad = "Casado y Gallo Pinto", PrecioPromedio = 8.00m, ImagenUrl = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=800" },
                new Restaurante { Nombre = "Sushi Sunset Tamarindo", TipoComida = "Japonesa / Fusion", Ubicacion = "Tamarindo, Guanacaste", Especialidad = "Rolls de Atún y Sashimi", PrecioPromedio = 30.00m, ImagenUrl = "https://images.unsplash.com/photo-1579584425555-c3ce17fd4351?w=800" },
                new Restaurante { Nombre = "Pizzería Bella Vista", TipoComida = "Italiana", Ubicacion = "Flamingo, Guanacaste", Especialidad = "Pizza Napolitana en Horno de Leña", PrecioPromedio = 15.00m, ImagenUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=800" },
                new Restaurante { Nombre = "Café Orgánico La Montaña", TipoComida = "Café y Repostería", Ubicacion = "Santa Cruz, Guanacaste", Especialidad = "Café de Especialidad y Postres Artesanales", PrecioPromedio = 6.00m, ImagenUrl = "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=800" },
                new Restaurante { Nombre = "Taco Loco Beach", TipoComida = "Mexicana", Ubicacion = "Playa Grande, Guanacaste", Especialidad = "Tacos al Pastor y Guacamole Fresco", PrecioPromedio = 12.00m, ImagenUrl = "https://images.unsplash.com/photo-1565299585323-38d6b0865b47?w=800" },
                new Restaurante { Nombre = "Parrilla Argentina El Gaucho", TipoComida = "Argentina", Ubicacion = "Sámara, Guanacaste", Especialidad = "Bife de Chorizo y Empanadas", PrecioPromedio = 22.00m, ImagenUrl = "https://images.unsplash.com/photo-1558030006-450675393462?w=800" },
                new Restaurante { Nombre = "Mar Azul Seafood", TipoComida = "Mariscos Gourmet", Ubicacion = "Papagayo, Guanacaste", Especialidad = "Pulpo a la Parrilla y Risotto de Mariscos", PrecioPromedio = 35.00m, ImagenUrl = "https://images.unsplash.com/photo-1587314168485-3236d6710814?w=800" },
                new Restaurante { Nombre = "Veggie Paradise", TipoComida = "Vegetariana / Vegana", Ubicacion = "Nosara, Guanacaste", Especialidad = "Bowls Orgánicos y Smoothies", PrecioPromedio = 14.00m, ImagenUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=800" }
            };

            var ofertas = new List<Oferta>
            {
                new Oferta { Titulo = "Paquete Romance - 3 Noches en Tamarindo", Descripcion = "Incluye 3 noches en Beach Resort, cena romántica, tour de snorkel y masaje de pareja.", Precio = 450.00m, FechaInicio = new DateTime(2025, 1, 1), FechaFin = new DateTime(2026, 1, 1) },
                new Oferta { Titulo = "Aventura Extrema - 5 Días", Descripcion = "Paquete completo: rafting, tirolesa, surf y tour en ATV. Hospedaje incluido.", Precio = 680.00m, FechaInicio = new DateTime(2025, 1, 1), FechaFin = new DateTime(2026, 1, 1) },
                new Oferta { Titulo = "Escapada Familiar - Semana Santa", Descripcion = "7 noches para 4 personas. Incluye tours educativos y actividades para niños.", Precio = 1200.00m, FechaInicio = new DateTime(2025, 1, 1), FechaFin = new DateTime(2026, 1, 1) },
                new Oferta { Titulo = "Relax Total - Retiro de Wellness", Descripcion = "4 noches con yoga diario, spa, alimentación orgánica y meditación.", Precio = 550.00m, FechaInicio = new DateTime(2025, 1, 1), FechaFin = new DateTime(2026, 1, 1) },
                new Oferta { Titulo = "Surf Camp - 2 Semanas", Descripcion = "Alojamiento + clases de surf diarias + equipo incluido. Perfecto para principiantes.", Precio = 890.00m, FechaInicio = new DateTime(2025, 1, 1), FechaFin = new DateTime(2026, 1, 1) },
                new Oferta { Titulo = "Luna de Miel en Papagayo", Descripcion = "5 noches en resort de lujo, desayunos incluidos, cena romántica y excursión en catamarán.", Precio = 1500.00m, FechaInicio = new DateTime(2025, 1, 1), FechaFin = new DateTime(2026, 1, 1) },
                new Oferta { Titulo = "Tour Fotográfico - 3 Días", Descripcion = "Paquete especializado para fotógrafos: atardeceres, vida silvestre y paisajes.", Precio = 420.00m, FechaInicio = new DateTime(2025, 1, 1), FechaFin = new DateTime(2026, 1, 1) },
                new Oferta { Titulo = "Black Friday - 40% OFF Hospedajes", Descripcion = "Descuento especial en todos los hospedajes. Válido para reservas de diciembre a marzo.", Precio = 299.00m, FechaInicio = new DateTime(2025, 1, 1), FechaFin = new DateTime(2026, 1, 1) },
                new Oferta { Titulo = "Aventura en Volcán Arenal", Descripcion = "2 noches + tours a cataratas, aguas termales y caminata nocturna.", Precio = 380.00m, FechaInicio = new DateTime(2025, 1, 1), FechaFin = new DateTime(2026, 1, 1) },
                new Oferta { Titulo = "Paquete Senior - Descuento Especial", Descripcion = "Para personas mayores de 65 años. Incluye hospedaje, tours tranquilos y alimentación.", Precio = 650.00m, FechaInicio = new DateTime(2025, 1, 1), FechaFin = new DateTime(2026, 1, 1) }
            };

            var transportes = new List<Transporte>
            {
                new Transporte { Tipo = "Shuttle Compartido", Empresa = "Interbus Costa Rica", Precio = 35.00m, ImagenUrl = "https://images.unsplash.com/photo-1544620347-c4fd4a3d5957?w=800" },
                new Transporte { Tipo = "Taxi Privado", Empresa = "Taxi Guanacaste Express", Precio = 80.00m, ImagenUrl = "https://images.unsplash.com/photo-1449965408869-eaa3f722e40d?w=800" },
                new Transporte { Tipo = "Renta de Auto (SUV)", Empresa = "Budget Rent a Car", Precio = 65.00m, ImagenUrl = "https://images.unsplash.com/photo-1519641471654-76ce0107ad1b?w=800" },
                new Transporte { Tipo = "Vuelo Doméstico", Empresa = "Sansa Airlines", Precio = 120.00m, ImagenUrl = "https://images.unsplash.com/photo-1436491865332-7a61a109cc05?w=800" }
            };

            await context.Hospedajes.AddRangeAsync(hospedajes);
            await context.Tours.AddRangeAsync(tours);
            await context.Restaurantes.AddRangeAsync(restaurantes);
            await context.Ofertas.AddRangeAsync(ofertas);
            await context.Transportes.AddRangeAsync(transportes);

            await context.SaveChangesAsync();
        }
    }
}
