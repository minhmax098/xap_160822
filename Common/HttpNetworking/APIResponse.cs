using System;

[Serializable]
public class APIResponse<T>
{
    public int code;
    public string message;
    public T data;
    public MetaData meta;
}

[Serializable]
public class MetaData
{
    public int page;
    public int pageSize;
    public int totalPage;
    public int totalElements;
}