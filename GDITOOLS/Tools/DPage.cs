using System;
using System.Collections.Generic;

namespace DTTOOLS.Tools
{
/// <summary>
/// 翻页管理类
/// </summary>
/// <typeparam name="T"></typeparam>
public class PageMode<T> 
{

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="index">当前页</param>
    /// <param name="count">总页数</param>
    /// <param name="span">翻页</param>
    /// <param name="data"></param>
    public PageMode(int span, List<T> data)
    {
        Pageindex = 0;

        Pagecount = (int) Math.Ceiling((double) data.Count/span);

        PageSpan = span;

        PageData = data;
    }

    /// <summary>
    /// 当前页
    /// </summary>
    public int Pageindex { set; get; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int Pagecount { set; get; }

    /// <summary>
    /// 每页的数据个数
    /// </summary>
    public int PageSpan { set; get; }

    /// <summary>
    /// 数据
    /// </summary>
    public List<T> PageData { set; get; }

    /// <summary>
    /// 跳转指定页数
    /// </summary>
    /// <param name="index">指定页</param>
    /// <returns>List<T></T></returns>
    public List<T> PageGo(int index)
    {
        if (Pagecount <= 0 || Pageindex >= Pagecount || Pageindex < 0) return new List<T>();


        var rList = new List<T>();

        for (int i = 0; i < PageSpan && (index)*PageSpan + i < PageData.Count; i++)
        {

            rList.Add(PageData[(index)*PageSpan + i]);
        }

        Pageindex = index;

        return rList;
    }

    /// <summary>
    /// 后翻页
    /// </summary>
    /// <returns>List<T></T></returns>
    public List<T> NextPage()
    {
        if (Pagecount <= 0 || Pageindex + 1 >= Pagecount) return new List<T>();

        return PageGo(Pageindex + 1);
    }

    /// <summary>
    /// 前翻页
    /// </summary>
    /// <returns>List<T></T></returns>
    public List<T> AgoPage()
    {
        if (Pagecount <= 0 || Pageindex - 1 < 0) return new List<T>();

        return PageGo(Pageindex - 1);
    }
}
}
