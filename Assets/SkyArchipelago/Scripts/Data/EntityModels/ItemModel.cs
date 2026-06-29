public class ItemModel : BaseModel<ItemData>
{
    public virtual CtxFlag GetTag => _dataModel.tag;

    public override string ModelName => $"Some Item";
}