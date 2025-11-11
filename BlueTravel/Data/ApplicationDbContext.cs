using BlueTravel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlueTravel.Data
{    // 👇 Hereda de IdentityDbContext para incluir usuarios y roles
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hospedaje> Hospedajes { get; set; }
        public DbSet<Oferta> Ofertas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Resena> Resenas { get; set; }
        public DbSet<Restaurante> Restaurantes { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Transporte> Transportes { get; set; }
        public DbSet<Reserva> Reservas { get; set; } // ✅ Nuevo
    }
}
