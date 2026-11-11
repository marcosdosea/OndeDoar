using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace DoadorWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddTransient<IDoadorService, DoadorService>();
            builder.Services.AddTransient<IAvaliacaoService, AvaliacaoService>();
            builder.Services.AddTransient<INecessidadeService, NecessidadeService>();

            builder.Services.AddTransient<IInstituicaoService, InstituicaoService>();

            builder.Services.AddTransient<IPostagemService, PostagemService>();
            builder.Services.AddTransient<INotificacaoService, NotificacaoService>();

            builder.Services.AddDbContext<OndeDoarContext>(
            options => options.UseMySQL(builder.Configuration.GetConnectionString("OndeDoarConnection") ?? throw new InvalidOperationException("Connection string 'OndeDoarConnection' not found.")));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
