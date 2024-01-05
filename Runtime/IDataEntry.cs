public interface IDataEntry<in T>
{
    void OnDataChanged(T data);
    void Refresh();
}