public abstract class MasterDetailView<TMaster, TDetail> : DataView<TDetail>, IDataEntry<TMaster>
{
    protected TMaster MasterData;

    public void OnDataChanged(TMaster data)
    {
        MasterData = data;
        Init();
    }
}