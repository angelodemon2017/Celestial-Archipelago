public interface BaseCatalogElementConfig<T>
    where T : struct
{
    public T UidKeyOfCatalog { get; set; }
}