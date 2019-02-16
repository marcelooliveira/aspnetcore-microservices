using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Model
{
    public class RedisBasketRepository : IBasketRepository
    {
        private const int BASKET_DB_INDEX = 0;
        private readonly ILogger<RedisBasketRepository> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisBasketRepository(ILogger<RedisBasketRepository> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            _database = redis.GetDatabase(BASKET_DB_INDEX);
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }

        public async Task<BasketCliente> GetBasketAsync(string clienteId)
        {
            if (string.IsNullOrWhiteSpace(clienteId))
                throw new ArgumentException();

            var data = await _database.StringGetAsync(clienteId);
            if (data.IsNullOrEmpty)
            {
                return await UpdateBasketAsync(new BasketCliente(clienteId));
            }
            return JsonConvert.DeserializeObject<BasketCliente>(data);
        }

        public IEnumerable<string> GetUsuarios()
        {
            var server = GetServer();
            return server.Keys()?.Select(k => k.ToString());
        }

        public async Task<BasketCliente> UpdateBasketAsync(BasketCliente basket)
        {
            var criado = await _database.StringSetAsync(basket.ClienteId, JsonConvert.SerializeObject(basket));
            if (!criado)
            {
                _logger.LogError("Erro ao atualizar o basket.");
                return null;
            }
            _logger.LogInformation("Basket atualizado.");
            return await GetBasketAsync(basket.ClienteId);
        }

        public async Task<BasketCliente> AddBasketAsync(string clienteId, ItemBasket item)
        {
            if (item == null)
                throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(item.ProdutoId))
                throw new ArgumentException();

            if (item.Quantidade <= 0)
                throw new ArgumentOutOfRangeException();

            var basket = await GetBasketAsync(clienteId);
            ItemBasket itemDB = basket.Itens.Where(i => i.ProdutoId == item.ProdutoId).SingleOrDefault();
            if (itemDB == null)
            {
                itemDB = new ItemBasket(item.Id, item.ProdutoId, item.ProdutoNome, item.PrecoUnitario, item.Quantidade);
                basket.Itens.Add(item);
            }
            return await UpdateBasketAsync(basket);
        }

        public async Task<UpdateQuantidadeOutput> UpdateBasketAsync(string clienteId, UpdateQuantidadeInput item)
        {
            if (item == null)
                throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(item.ProdutoId))
                throw new ArgumentException();

            if (item.Quantidade < 0)
                throw new ArgumentOutOfRangeException();

            var basket = await GetBasketAsync(clienteId);
            ItemBasket itemDB = basket.Itens.Where(i => i.ProdutoId == item.ProdutoId).SingleOrDefault();
            itemDB.Quantidade = item.Quantidade;
            if (item.Quantidade == 0)
            {
                basket.Itens.Remove(itemDB);
            }
            BasketCliente basketCliente = await UpdateBasketAsync(basket);
            return new UpdateQuantidadeOutput(itemDB, basketCliente);
        }

        private IServer GetServer()
        {
            var endpoints = _redis.GetEndPoints();
            return _redis.GetServer(endpoints.First());
        }

    }
}
