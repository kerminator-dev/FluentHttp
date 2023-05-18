namespace FluentHTTPToolkit.Core.Builders.Interfaces
{
    public interface IBuilder<TModel>
    {
        Task<TModel> Build();
    }
}
