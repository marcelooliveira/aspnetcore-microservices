using Models.ViewModels;

namespace MVC
{
    public class UserManager
    {
        public static ApplicationUser GetUser()
        {
            return new ApplicationUser
            {
                Id = "123",
                UserName = "alice",
                Email = "alice@asp.net",
                Name = "Alice Smith",
                Phone = "1234-5678",
                Address = "Rua Vergueiro, 456",
                AdditionalAddress = "8 andar sala 801",
                District = "Vila Mariana",
                City = "São Paulo",
                State = "SP",
                ZipCode = "69118"
            };
        }
    }
}
