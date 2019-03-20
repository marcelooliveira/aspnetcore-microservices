using System.Threading.Tasks;

namespace MVC
{
    public interface ISessionHelper
    {
        int? GetOrderId();
        void SetOrderId(int orderId);
    }
}