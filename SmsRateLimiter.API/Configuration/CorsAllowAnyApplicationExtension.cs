namespace Mapper.GSB.Rest.API.StartupConfig.Applications
{
    public static class CorsAllowAnyApplicationExtension
    {
        public static void UseCorsAllowAny(this IApplicationBuilder App)
        {
            App.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }
    }
}
