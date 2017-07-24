using System.Threading.Tasks;

namespace ApiAuth.Models {
    public interface IMessageService {
        Task Send (string email, string subject, string message);

    }
}