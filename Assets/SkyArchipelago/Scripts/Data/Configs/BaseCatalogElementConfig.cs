public interface BaseCatalogElementConfig<T>
    where T : struct
{
    public T KeyOfCatalog { get; set; }
}