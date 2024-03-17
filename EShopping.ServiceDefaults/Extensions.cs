using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using System;

namespace Microsoft.Extensions.Hosting
{
    public static class Extensions
    {
        public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
        {
            builder.ConfigureOpenTelemetry();

            builder.AddDefaultHealthChecks();

            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                http.UseServiceDiscovery();
            });

            return builder;
        }

        public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddRuntimeInstrumentation()
                           .AddBuiltInMeters();
                })
                .WithTracing(tracing =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        // We want to view all traces in development
                        tracing.SetSampler(new AlwaysOnSampler());
                    }

                    tracing.AddAspNetCoreInstrumentation()
                           .AddGrpcClientInstrumentation()
                           .AddHttpClientInstrumentation();
                });

            builder.AddOpenTelemetryExporters();

            return builder;
        }

        private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
        {
            var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
            LoggerConfiguration logBuilder = new LoggerConfiguration();
            ConfigureService.Invoke(() => Convert.ToBoolean(builder.Configuration["ConfigureSerilog"]) == true, () =>
            {
                logBuilder = AddSerilog(builder);
            });
            if (useOtlpExporter)
            {
                ConfigureService.Invoke(() => Convert.ToBoolean(builder.Configuration["ConfigureSerilog"]) == false, () =>
                {
                    builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter());
                });
          
                ConfigureService.Invoke(() => Convert.ToBoolean(builder.Configuration["ConfigureSerilog"]) == true, () =>
                {
                    SinkSeriLogToOpenTelemetry(builder, logBuilder);
                });
                builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter());
                builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());
            }

            ConfigureService.Invoke(() => Convert.ToBoolean(builder.Configuration["ConfigureSerilog"]) == true, () =>
            {
                Log.Logger = logBuilder.CreateBootstrapLogger();

                builder.Logging.AddSerilog();
            });
           
            // Uncomment the following lines to enable the Prometheus exporter (requires the OpenTelemetry.Exporter.Prometheus.AspNetCore package)
            // builder.Services.AddOpenTelemetry()
            //    .WithMetrics(metrics => metrics.AddPrometheusExporter());

            // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
            // builder.Services.AddOpenTelemetry()
            //    .UseAzureMonitor();

            return builder;
        }

        private static void SinkSeriLogToOpenTelemetry(IHostApplicationBuilder builder, LoggerConfiguration logBuilder)
        {
            logBuilder
               .WriteTo.OpenTelemetry(options =>
               {
                   options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
                   options.ResourceAttributes.Add("service.name", builder.Environment.ApplicationName.ToLower());
               });
        }

        private static LoggerConfiguration AddSerilog(IHostApplicationBuilder builder)
        {
            var logBuilder = new LoggerConfiguration()
                                           .MinimumLevel.Information()
                                            .Enrich.FromLogContext()
                                            .Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName)

                                            .Enrich.WithProperty("EnvironmentName", builder.Environment.ApplicationName)
                                            // .Enrich.WithExceptionDetails()
                                            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                                            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                                            .WriteTo.Console();
            if (builder.Environment.IsDevelopment())
            {
                logBuilder.MinimumLevel.Override("Catalog", LogEventLevel.Debug);
                logBuilder.MinimumLevel.Override("Basket", LogEventLevel.Debug);
                logBuilder.MinimumLevel.Override("Discount", LogEventLevel.Debug);
                logBuilder.MinimumLevel.Override("Ordering", LogEventLevel.Debug);
            }

            return logBuilder;
        }

        public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                // Add a default liveness check to ensure app is responsive
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            // Uncomment the following line to enable the Prometheus endpoint (requires the OpenTelemetry.Exporter.Prometheus.AspNetCore package)
            // app.MapPrometheusScrapingEndpoint();

            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });

            return app;
        }

        private static MeterProviderBuilder AddBuiltInMeters(this MeterProviderBuilder meterProviderBuilder) =>
            meterProviderBuilder.AddMeter(
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Server.Kestrel",
                "System.Net.Http");

        private static readonly Action<Func<bool>, Action> ConfigureService = (Func<bool> predicate, Action service) =>
        {
            if (predicate.Invoke())
            {
                service.Invoke();
            }
        };
    }
}
