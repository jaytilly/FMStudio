using FMStudio.App.Utility;

namespace FMStudio.App.ViewModels
{
    public interface IHaveAName
    {
        Binding<string> Name { get; }
    }
}