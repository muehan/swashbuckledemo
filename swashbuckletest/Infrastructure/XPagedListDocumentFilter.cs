using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using X.PagedList;

namespace swashbuckletest.Infrastructure
{
    public class XPagedListDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            List<string> models = FindPagedListModels(context);

            if (models.Any())
            {
                ChangeOutputForPageListModels(swaggerDoc, models);
            }
        }

        private void ChangeOutputForPageListModels(SwaggerDocument swaggerDoc, List<string> models)
        {
            var definitions = swaggerDoc.Definitions.Where(x => models.Contains(x.Key));

            var newdefinitions = new Dictionary<string, Schema>();

            foreach (var definition in definitions)
            {
                string modelName = definition.Key;

                newdefinitions.Add($"{modelName}PagedList", GetPagedListSchema(modelName));

                ChangePathsForModelName(swaggerDoc, modelName);
            }

            foreach (var def in newdefinitions)
            {
                swaggerDoc.Definitions.Add(def);
            }
        }

        private void ChangePathsForModelName(SwaggerDocument swaggerDoc, string modelName)
        {
            foreach (var path in swaggerDoc.Paths)
            {
                if (path.Value.Get?.Responses != null &&
                    path.Value.Get.Responses.ContainsKey("200") &&
                    path.Value.Get.Responses["200"].Schema?.Items != null &&
                    path.Value.Get.Responses["200"].Schema.Items.Ref == $"#/definitions/{modelName}")
                {
                    path.Value.Get.Responses["200"].Schema.Ref = $"#/definitions/{modelName}PagedList";
                    path.Value.Get.Responses["200"].Schema.Type = null;
                }
            }
        }

        private List<string> FindPagedListModels(DocumentFilterContext context)
        {
            var models = new List<string>();

            // api version 1
            foreach (var versionGroup in context.ApiDescriptionsGroups.Items)
            {
                FindAllModelsFromGetResponses(models, versionGroup);
            }

            return models;
        }

        private void FindAllModelsFromGetResponses(List<string> models, ApiDescriptionGroup versionGroup)
        {
            // items in current API Version
            foreach (ApiDescription description in GetAllGetMethods(versionGroup))
            {
                var response = description.SupportedResponseTypes.SingleOrDefault(x => x.StatusCode == 200 && x.Type.Namespace.ToLower().Contains("x.pagedlist"));

                if (response != null)
                {
                    models.Add(response.Type.GenericTypeArguments.First().Name);
                }
            }
        }

        private IEnumerable<ApiDescription> GetAllGetMethods(ApiDescriptionGroup versionGroup)
        {
            return versionGroup.Items.Where(x => x.HttpMethod.ToLower() == "get");
        }

        private Schema GetPagedListSchema(string modelName)
        {
            var properties = new Dictionary<string, Schema>();
            var pagedCust = typeof(IPagedList);

            foreach (PropertyInfo property in pagedCust.GetTypeInfo().GetProperties())
            {
                var schema = new Schema();
                properties.Add(property.Name.FirstCharToLowerCase(), schema);
            }

            var itemsSchema = new Schema {Type = "array", Items = new Schema {Ref = $"#/definitions/{modelName}"}};
            properties.Add("items", itemsSchema);

            return new Schema {Properties = properties};
        }
    }
}
