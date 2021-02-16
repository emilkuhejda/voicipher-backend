using System.Threading.Tasks;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IWavFileService
    {
        Task RunConversionToWavAsync();
    }
}
