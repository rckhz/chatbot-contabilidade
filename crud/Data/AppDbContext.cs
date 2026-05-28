using Microsoft.EntityFrameworkCore;
using crud.Models;

namespace crud.Data
{
    public class AppDbContext : DbContext
    {

        //recebe a confiragacao do banco de dados
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        //cria as tabelas do banco de dados a partir das classes criadas
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
    }
}