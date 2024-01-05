using System.Collections.Generic;

public interface IDataView<out T>
{
    IEnumerable<T> EnumerateData();
    void Refresh();
}