namespace FMStudio.App.Interfaces
{
    public interface ICanBeDroppedUpon
    {
        void Drop(ICanBeDragged draggable);
    }
}