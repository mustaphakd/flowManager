using Analyzer.Models;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public interface ISettingsService
    {
        SettingsModel GetSettings();
        Task SetSettings(SettingsModel model);
    }
}