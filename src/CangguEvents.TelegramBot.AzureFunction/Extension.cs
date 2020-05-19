using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace CangguEvents.TelegramBot.AzureFunction
{
    static class Extension
    {
        static IEnumerable<AssemblyName> EnumerateAllAssemblies()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            yield return executingAssembly.GetName();

            foreach (var assembly in executingAssembly.GetReferencedAssemblies())
            {
                yield return assembly;
            }
        }

        public static void RegisterAutomapper(this ContainerBuilder builder)
        {
            var autoMapperProfiles = EnumerateAllAssemblies()
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