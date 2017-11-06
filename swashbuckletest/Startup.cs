namespace swashbuckletest
{
    using Infrastructure;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Swagger;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Collections.Generic;
    using System.Reflection;
    using X.PagedList;   

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddControllersAsServices().AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                options.SerializerSettings.Converters.Add(new CustomIEnumerableConverter(new[] { "X.PagedList" }));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.DocumentFilter<PagedListDocumentFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.DocExpansion("full");
            });
        }

        private class PagedListDocumentFilter : IDocumentFilter
        {            
            public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
            {
                if (swaggerDoc.Definitions != null && swaggerDoc.Definitions.ContainsKey("Customer"))
                {
                    swaggerDoc.Definitions.Add("CustomerPagedList", PagedListSchema);
                }
                foreach (var path in swaggerDoc.Paths)
                {
                    if (path.Value.Get != null && path.Value.Get.Responses != null && path.Value.Get.Responses.ContainsKey("200"))
                        if (path.Value.Get.Responses["200"].Schema.Items.Ref == "#/definitions/Customer")
                            path.Value.Get.Responses["200"].Schema.Items.Ref = "#/definitions/CustomerPagedList";
                }
            }

            private Schema PagedListSchema
            {
                get
                {
                    var data = new Dictionary<string, Schema>();
                    var pagedCust = typeof(IPagedList);
                    foreach (PropertyInfo property in pagedCust.GetTypeInfo().GetProperties())
                    {
                        var sch = new Schema();
                        data.Add(property.Name, sch);
                    }
                    var s = new Schema { Type = "array", Items = new Schema { Ref = "#/definitions/Customer" } };
                    data.Add("items", s);
                    return new Schema { Properties = data };
                }
            }
        }
    }
}
