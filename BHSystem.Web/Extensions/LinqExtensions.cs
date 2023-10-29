using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BHSystem.Web.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Checks if a value is string or any other object if it is string
        /// it checks for nullorwhitespace otherwhise it checks for null only
        /// </summary>
        /// <typeparam name="T">Type of the item you want to check</typeparam>
        /// <param name="item">The item you want to check</param>
        /// <param name="nameOfTheArgument">Name of the argument</param>
        public static void IsNullorWhiteSpace<T>(T item, string nameOfTheArgument = "")
        {

            Type type = typeof(T);
            if (type == typeof(string) ||
                type == typeof(string))
            {
                if (string.IsNullOrWhiteSpace(item as string))
                {
                    throw new ArgumentException(nameOfTheArgument + " is null or Whitespace");
                }
            }
            else
            {
                if (item == null)
                {
                    throw new ArgumentException(nameOfTheArgument + " is null");
                }
            }

        }

        /// <summary>
        /// Câp nhật giá tri phần tử trong collection
        /// </summary>
        /// <param name="enumerable">Giá tri muốn thay đổi LinQ</param>
        /// <param name="update"></param>
        /// <returns>
        /// trả về IEnumable => chuyển sang List
        /// </returns>
        public static IEnumerable<T> Update<T>(this IEnumerable<T> enumerable,
                                               Action<T> update) where T : class
        {
            IsNullorWhiteSpace(enumerable, "enumerable");
            IsNullorWhiteSpace(update, "update");
            foreach (var item in enumerable)
            {
                update(item);
            }
            return enumerable;
        }

        /// <summary>
        /// Câp nhật giá tri phần tử trong collection
        /// where theo điều kiện
        /// </summary>
        /// <param name="enumerable">Giá tri muốn thay đổi LinQ</param>
        /// <param name="update"></param>
        /// <param name="where">Điều kiện trong danh sách</param>
        /// <returns>
        /// trả về IEnumable => chuyển sang List
        /// </returns>
        public static IEnumerable<T> UpdateWhere<T>(this IEnumerable<T> enumerable,
                                               Action<T> update, Func<T, bool> where) where T : class
        {
            IsNullorWhiteSpace(enumerable, "enumerable");
            IsNullorWhiteSpace(update, "update");
            foreach (var item in enumerable)
            {
                if (where(item))
                {
                    update(item);
                }
            }
            return enumerable;
        }
    }
}