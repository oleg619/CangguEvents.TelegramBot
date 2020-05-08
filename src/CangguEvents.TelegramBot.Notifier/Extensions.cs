using System;
using System.Linq;
using System.Reflection;
using Autofac;
using AutoMapper;
using CangguEvents.Domain.Extensions;

namespace CangguEvents.TelegramBot.Notifier
{
    public static class Extensions
    {
        public static void RegisterAutomapper(this ContainerBuilder builder)
        {
            var autoMapperProfiles = Assembly.GetExecutingAssembly().EnumerateAllAssemblies()
                .SelectMany(an => Assembly.Load(an).GetTypes())
                .Where(p => typeof(Profile).IsAssignableFrom(p) && p.IsPublic && !p.IsAbstract)
                .Distinct()
                .Select(p => Activator.CreateInstance(p) as Profile);

            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                foreach (var profile in autoMapperProfiles)
                {
                    cfg.AddProfile(profile);
                }
            }));

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>()
                .InstancePerLifetimeScope();
        }
    }
}