using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _client;

        public CatalogService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            HttpResponseMessage response = await _client.GetAsync("/api/v1/Catalog");

            IEnumerable<CatalogModel>? otherResponse = await _client
                .GetFromJsonAsync<IEnumerable<CatalogModel>>("/api/v1/Catalog");

            return await response.ReadContentAs<IEnumerable<CatalogModel>>();
        }

        public async Task<CatalogModel> GetCatalog(string id)
        {
            HttpResponseMessage response = await _client.GetAsync($"/api/v1/Catalog/{id}");
            return await response.ReadContentAs<CatalogModel>();
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
        {
            HttpResponseMessage response = await _client.GetAsync($"/api/v1/Catalog/GetProductByCategory/{category}");
            return await response.ReadContentAs<IEnumerable<CatalogModel>>();
        }
    }
}
