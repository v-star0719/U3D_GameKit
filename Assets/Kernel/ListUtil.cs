using System.Collections;
using System.Collections.Generic;

namespace GameKit.Kernel
{
    public class ListUtil
    {
        /// <summary>
        /// 一个列表里的项移上移下
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="item">项</param>
        /// <param name="isMoveUp">true = 上移，false = 下移</param>
        public static void MoveUpDown<T>(List<T> list, T item, bool isMoveUp)
        {
            if (list.Count <= 1)
            {
                return;
            }

            int index = list.IndexOf(item);

            if (index < 0 || index >= list.Count)
            {
                return;
            }

            T t = list[index];
            int targetIndex = 0;
            if (index == 0)
            {
                if (isMoveUp)
                {
                    //不能进行交换，直接插入到最后
                    list.RemoveAt(index);
                    list.Add(t);
                    return;
                }

                targetIndex = 1;
            }
            else if (index == list.Count - 1)
            {

                if (!isMoveUp)
                {
                    //不能进行交换，直接插入到最前
                    list.RemoveAt(index);
                    list.Insert(0, t);
                    return;
                }

                targetIndex = list.Count - 2;
            }
            else
            {
                targetIndex = index + (isMoveUp ? -1 : 1);
            }

            list[index] = list[targetIndex];
            list[targetIndex] = t;
        }
    }
}