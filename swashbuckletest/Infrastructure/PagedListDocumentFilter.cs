namespace swashbuckletest.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    public class PagedListDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            Schema customers = swaggerDoc.Definitions["Customer"];
            customers.Description = "Custom Customer description";

            IDictionary<string, Schema> typeProperties = customers.Properties;
            customers.Properties = new Dictionary<string, Schema>();
            customers.Properties.Add("count", new Schema { Type = "integer", Format = "int32"});
            customers.Properties.Add("pageCount", new Schema { Type = "integer", Format = "int32"});
            customers.Properties.Add("totalItemCount", new Schema { Type = "integer", Format = "int32"});
            customers.Properties.Add("pageNumber", new Schema { Type = "integer", Format = "int32"});
            customers.Properties.Add("pageSize", new Schema { Type = "integer", Format = "int32"});
            customers.Properties.Add("hasPreviousPage", new Schema { Type = "boolean", Format = "bool"});
            customers.Properties.Add("hasNextPage", new Schema { Type = "boolean", Format = "bool"});
            customers.Properties.Add("isFirstPage", new Schema { Type = "boolean", Format = "bool"});
            customers.Properties.Add("isLastPage", new Schema { Type = "boolean", Format = "bool" });
            customers.Properties.Add("firstItemOnPagePage", new Schema { Type = "integer", Format = "int32" });
            customers.Properties.Add("lastItemOnPage", new Schema { Type = "integer", Format = "int32" });
            customers.Properties.Add("items", new Schema { Properties = typeProperties });
            
        }
    }
}
