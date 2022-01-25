using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenApiInterpreter
{
    public class OpenApiInterface
    {
        protected OpenApiDocument openApiDocument { get; private set; } = new OpenApiDocument();

        private OpenApiDiagnostic diagnostic = new OpenApiDiagnostic();
        /// <summary>
        /// Convenicence property
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return diagnostic.Errors.Count > 0;
            }
        }
        /// <summary>
        /// Convenience property
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Errors
        {
            get
            {
                return diagnostic.Errors.Select(oae => oae.Message);
            }
        }
        public async Task ReadAsync(string openApiDefinition)
        {
            await Task.Run(() =>
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var stream = httpClient.GetStreamAsync(openApiDefinition);

                        // Read V3 as YAML
                        openApiDocument = new OpenApiStreamReader().Read(stream.Result, out diagnostic);
                    }
                    catch (AggregateException e)
                    {
                        diagnostic.Errors.Add(new OpenApiError("HTTP", e.InnerException.Message));
                    }
                }
            });
        }

        public List<string> GetPaths()
        {
            return openApiDocument.Paths.Select(openApiPathItem => openApiPathItem.Key).ToList();
        }

        internal IDictionary<OperationType, OpenApiOperation> GetMethods(string path)
        {
            return openApiDocument.Paths[path].Operations;
        }

        public void WriteAsJson()
        {
            // Write V2 as JSON
            var outputString = openApiDocument.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json);

            Console.WriteLine(outputString);
        }
    }
}
