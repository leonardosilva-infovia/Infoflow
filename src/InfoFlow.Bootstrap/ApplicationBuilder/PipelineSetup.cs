using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace InfoFlow.Bootstrap.ApplicationBuilder;

public static class PipelineSetup
{
  public static WebApplication UseInfoFlowPipeline(this WebApplication app, IHostEnvironment env)
  {
    app.UseSerilogRequestLogging();

    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseSwagger();
      app.UseSwaggerUI(opt =>
      {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        opt.DisplayRequestDuration();
      });
    }

    app.UseHttpsRedirection();
    app.UseCors("default");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");

    return app;
  }
}