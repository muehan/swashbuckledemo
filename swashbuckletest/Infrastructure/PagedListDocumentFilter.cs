namespace swashbuckletest.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using X.PagedList;

    public class PagedListDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            Schema customers = swaggerDoc.Definitions["Customer"];

            // read current properties from model
            IDictionary<string, Schema> listItemProperties = customers.Properties;

            // clear all properties
            customers.Properties = new Dictionary<string, Schema>();
            
            // reapply properties under "items" schema
            customers.Properties.Add("items", new Schema { Properties = listItemProperties });
            
            // all all IPagedList Properties
            AddPagedListProperties(customers.Properties);
        }

        private void AddPagedListProperties(IDictionary<string, Schema> customersProperties)
        {
            var properties = typeof(IPagedList).GetProperties();

            foreach (var propertyInfo in properties)
            {
                switch (propertyInfo.PropertyType.Name)
                {
                    case "Int32":
                        customersProperties.Add(propertyInfo.Name.FirstCharToLowerCase(), CreateIntegerSchema());
                        break;
                    case "String":
                        customersProperties.Add(propertyInfo.Name.FirstCharToLowerCase(), CreateStringSchema());
                        break;
                    case "Boolean":
                        customersProperties.Add(propertyInfo.Name.FirstCharToLowerCase(), CreateBooleanSchema());
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private Schema CreateBooleanSchema()
        {
            return new Schema { Format = "bool", Type = "boolean" };
        }

        private Schema CreateStringSchema()
        {
            return new Schema { Format = "string", Type = "string" };
        }

        private Schema CreateIntegerSchema()
        {
            return new Schema { Format = "int32", Type = "integer" };
        }
    }
}
