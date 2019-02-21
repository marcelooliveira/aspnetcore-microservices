using System.Threading.Tasks;

namespace CasaDoCodigo
{
    public interface ISessionHelper
    {
        Task<string> GetAccessToken(string scope);
        int? GetOrderId();
        void SetAccessToken(string accessToken);
        void SetOrderId(int orderId);
    }
}