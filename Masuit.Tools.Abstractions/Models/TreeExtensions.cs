﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Masuit.Tools.Systems;

namespace Masuit.Tools.Models
{
    /// <summary>
    /// 树形数据扩展
    /// </summary>
    public static class TreeExtensions
    {
        /// <summary>
        /// 过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> items, Func<T, bool> func) where T : class, ITreeChildren<T>
        {
            foreach (var item in items.Where(i => i != null))
            {
                item.Children ??= new List<T>();
                item.Children = item.Children.Filter(func).ToList();
                if (item.Children.Any() || func(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<Tree<T>> Filter<T>(this IEnumerable<Tree<T>> items, Func<Tree<T>, bool> func) where T : class
        {
            foreach (var item in items.Where(i => i != null))
            {
                item.Children ??= new List<Tree<T>>();
                item.Children = item.Children.Filter(func).ToList();
                if (item.Children.Any() || func(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<T> Filter<T>(this T item, Func<T, bool> func) where T : class, ITreeChildren<T>
        {
            return new[] { item }.Filter(func);
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<Tree<T>> Filter<T>(this Tree<T> item, Func<Tree<T>, bool> func) where T : class
        {
            return new[] { item }.Filter(func);
        }

        /// <summary>
        /// 平铺开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="optionAction">平铺时子级需要做的操作，参数1：子级对象，参数2：父级对象</param>
        /// <returns></returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> items, Action<T, T> optionAction = null) where T : class, ITreeChildren<T>
        {
            foreach (var item in items)
            {
                yield return item;
                item.Children ??= new List<T>();
                item.Children.ForEach(c => optionAction?.Invoke(c, item));
                foreach (var children in item.Children.Flatten(optionAction))
                {
                    yield return children;
                }
            }
        }

        /// <summary>
        /// 平铺开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <param name="optionAction">平铺时子级需要做的操作，参数1：子级对象，参数2：父级对象</param>
        /// <returns></returns>
        public static IEnumerable<T> Flatten<T>(this T p, Action<T, T> optionAction = null) where T : class, ITreeChildren<T>
        {
            yield return p;
            foreach (var item in p.Children)
            {
                yield return item;
                item.Children ??= new List<T>();
                item.Children.ForEach(c => optionAction?.Invoke(c, item));
                foreach (var children in item.Children.Flatten())
                {
                    yield return children;
                }
            }
        }

        /// <summary>
        /// 平铺开任意树形结构数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="selector"></param>
        /// <param name="optionAction">平铺时子级需要做的操作，参数1：子级对象，参数2：父级对象</param>
        /// <returns></returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> selector, Action<T, T> optionAction = null)
        {
            foreach (var item in items)
            {
                yield return item;
                selector(item).ForEach(c => optionAction?.Invoke(c, item));
                foreach (var children in selector(item).Flatten(selector))
                {
                    yield return children;
                }
            }
        }

        /// <summary>
        /// 平铺开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="optionAction">平铺时子级需要做的操作，参数1：子级对象，参数2：父级对象</param>
        /// <returns></returns>
        public static IEnumerable<Tree<T>> Flatten<T>(this IEnumerable<Tree<T>> items, Action<Tree<T>, Tree<T>> optionAction = null) where T : class
        {
            foreach (var item in items)
            {
                yield return item;
                item.Children ??= new List<Tree<T>>();
                item.Children.ForEach(c => optionAction?.Invoke(c, item));
                foreach (var tree in item.Children.Flatten())
                {
                    yield return tree;
                }
            }
        }

        /// <summary>
        /// 平铺开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <param name="optionAction">平铺时子级需要做的操作，参数1：子级对象，参数2：父级对象</param>
        /// <returns></returns>
        public static IEnumerable<Tree<T>> Flatten<T>(this Tree<T> p, Action<Tree<T>, Tree<T>> optionAction = null) where T : class
        {
            yield return p;
            foreach (var item in p.Children)
            {
                yield return item;
                item.Children ??= new List<Tree<T>>();
                item.Children.ForEach(c => optionAction?.Invoke(c, item));
                foreach (var tree in item.Children.Flatten())
                {
                    yield return tree;
                }
            }
        }

        /// <summary>
        /// 平铺开任意树形结构数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="selector"></param>
        /// <param name="optionAction">平铺时子级需要做的操作，参数1：子级对象，参数2：父级对象</param>
        /// <returns></returns>
        public static IEnumerable<Tree<T>> Flatten<T>(this IEnumerable<Tree<T>> items, Func<Tree<T>, IEnumerable<Tree<T>>> selector, Action<Tree<T>, Tree<T>> optionAction = null)
        {
            foreach (var item in items)
            {
                yield return item;
                item.Children.ForEach(c => optionAction?.Invoke(c, item));
                foreach (var tree in selector(item).Flatten(selector))
                {
                    yield return tree;
                }
            }
        }

        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="idSelector"></param>
        /// <param name="pidSelector"></param>
        /// <param name="topValue">根对象parentId的值</param>
        /// <returns></returns>
        public static List<T> ToTree<T>(this IEnumerable<T> source, Expression<Func<T, string>> idSelector, Expression<Func<T, string>> pidSelector, string topValue = default) where T : ITreeParent<T>, ITreeChildren<T>
        {
            return ToTree<T, string>(source, idSelector, pidSelector, topValue);
        }

        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="idSelector"></param>
        /// <param name="pidSelector"></param>
        /// <param name="topValue">根对象parentId的值</param>
        /// <returns></returns>
        public static List<T> ToTree<T>(this IEnumerable<T> source, Expression<Func<T, int>> idSelector, Expression<Func<T, int>> pidSelector, int topValue = 0) where T : ITreeParent<T>, ITreeChildren<T>
        {
            return ToTree<T, int>(source, idSelector, pidSelector, topValue);
        }

        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="idSelector"></param>
        /// <param name="pidSelector"></param>
        /// <param name="topValue">根对象parentId的值</param>
        /// <returns></returns>
        public static List<T> ToTree<T>(this IEnumerable<T> source, Expression<Func<T, long>> idSelector, Expression<Func<T, long>> pidSelector, long topValue = 0) where T : ITreeParent<T>, ITreeChildren<T>
        {
            return ToTree<T, long>(source, idSelector, pidSelector, topValue);
        }

        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="idSelector"></param>
        /// <param name="pidSelector"></param>
        /// <param name="topValue">根对象parentId的值</param>
        /// <returns></returns>
        public static List<T> ToTree<T>(this IEnumerable<T> source, Expression<Func<T, Guid>> idSelector, Expression<Func<T, Guid>> pidSelector, Guid topValue = default) where T : ITreeParent<T>, ITreeChildren<T>
        {
            return ToTree<T, Guid>(source, idSelector, pidSelector, topValue);
        }

        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="idSelector"></param>
        /// <param name="pidSelector"></param>
        /// <param name="topValue">根对象parentId的值</param>
        /// <returns></returns>
        public static List<T> ToTree<T, TKey>(this IEnumerable<T> source, Expression<Func<T, TKey>> idSelector, Expression<Func<T, TKey>> pidSelector, TKey topValue = default) where T : ITreeParent<T>, ITreeChildren<T> where TKey : IComparable
        {
            if (source is IQueryable<T> queryable)
            {
                source = queryable.ToList();
            }

            if (idSelector.Body.ToString() == pidSelector.Body.ToString())
            {
                throw new ArgumentException("idSelector和pidSelector不应该为同一字段！");
            }

            var pidFunc = pidSelector.Compile();
            var idFunc = idSelector.Compile();
            return TransData(source.Where(t => t != null), idFunc, pidFunc, topValue).ToList();
        }

        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> ToTree<T>(this IEnumerable<T> source) where T : ITreeEntity<T, int>
        {
            return ToTree<T, int>(source);
        }

        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> ToTree<T, TKey>(this IEnumerable<T> source) where T : ITreeEntity<T, TKey> where TKey : struct, IComparable
        {
            if (source is IQueryable<T> queryable)
            {
                source = queryable.ToList();
            }

            source = source.Where(t => t != null).ToList();
            var temp = new List<T>();
            foreach (var item in source.Where(item => item.ParentId is null || item.ParentId.Equals(default)))
            {
                temp.AddRange(TransData<T, TKey>(source, item));
            }

            return temp;
        }

        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="idSelector"></param>
        /// <param name="pidSelector"></param>
        /// <param name="topValue">根对象parentId的值</param>
        /// <returns></returns>
        public static List<T> ToTree<T, TKey>(this IEnumerable<T> source, Expression<Func<T, TKey>> idSelector, Expression<Func<T, TKey?>> pidSelector, TKey? topValue = default) where T : ITreeChildren<T> where TKey : struct
        {
            if (source is IQueryable<T> queryable)
            {
                source = queryable.ToList();
            }

            var pidFunc = pidSelector.Compile();
            var idFunc = idSelector.Compile();
            source = source.Where(t => t != null).ToList();
            var temp = new List<T>();
            foreach (var item in source.Where(item => pidFunc(item) is null || pidFunc(item).Equals(topValue)))
            {
                temp.AddRange(TransData(source, item, idFunc, pidFunc));
            }

            return temp;
        }

        private static IEnumerable<T> TransData<T, TKey>(IEnumerable<T> source, Func<T, TKey> idSelector, Func<T, TKey> pidSelector, TKey topValue = default) where T : ITreeChildren<T> where TKey : IComparable
        {
            // 创建一个字典，用于快速查找节点的子节点
            var childrenLookup = new Dictionary<TKey, List<T>>();
            foreach (var item in source.Where(item => !childrenLookup.ContainsKey(idSelector(item))))
            {
                childrenLookup[idSelector(item)] = new List<T>();
            }

            // 构建树结构
            foreach (var item in source.Where(item => Equals(pidSelector(item), default(TKey)) && childrenLookup.ContainsKey(pidSelector(item))))
            {
                childrenLookup[pidSelector(item)].Add(item);
            }

            // 找到根节点，即没有父节点的节点
            foreach (var root in source.Where(x => Equals(pidSelector(x), topValue)))
            {
                // 为根节点和所有子节点设置Children属性
                // 使用队列来模拟递归过程
                var queue = new Queue<T>();
                queue.Enqueue(root);

                while (queue.Count > 0)
                {
                    // 出队当前节点
                    var current = queue.Dequeue();

                    // 为当前节点设置子节点
                    if (childrenLookup.TryGetValue(idSelector(current), out var children))
                    {
                        current.Children = children;
                        foreach (var child in children)
                        {
                            // 如果子节点实现了ITreeParent接口，则设置其Parent属性
                            if (child is ITreeParent<T> tree)
                            {
                                tree.Parent = current;
                            }

                            // 将子节点入队以继续处理
                            queue.Enqueue(child);
                        }
                    }
                }
                yield return root;
            }
        }

        internal static IEnumerable<T> TransData<T, TKey>(IEnumerable<T> source, T parent) where T : ITreeEntity<T, TKey> where TKey : struct, IComparable
        {
            // 创建一个字典，用于快速查找节点的子节点
            var childrenLookup = new Dictionary<TKey?, List<T>>();
            foreach (var item in source.Where(item => !childrenLookup.ContainsKey(item.Id)))
            {
                childrenLookup[item.Id] = new List<T>();
            }

            // 构建树结构
            foreach (var item in source.Where(item => Equals(item.ParentId, default(TKey)) && childrenLookup.ContainsKey(item.ParentId)))
            {
                childrenLookup[item.ParentId].Add(item);
            }

            // 找到根节点，即没有父节点的节点
            foreach (var root in source.Where(x => Equals(x.ParentId, parent.Id)))
            {
                // 为根节点和所有子节点设置Children属性
                // 使用队列来模拟递归过程
                var queue = new Queue<T>();
                queue.Enqueue(root);

                while (queue.Count > 0)
                {
                    // 出队当前节点
                    var current = queue.Dequeue();

                    // 为当前节点设置子节点
                    if (childrenLookup.TryGetValue(current.Id, out var children))
                    {
                        current.Children = children;
                        foreach (var child in children)
                        {
                            // 如果子节点实现了ITreeParent接口，则设置其Parent属性
                            if (child is ITreeParent<T> tree)
                            {
                                tree.Parent = current;
                            }

                            // 将子节点入队以继续处理
                            queue.Enqueue(child);
                        }
                    }
                }
                yield return root;
            }
        }

        internal static IEnumerable<T> TransData<T>(IEnumerable<T> source, T parent) where T : ITreeEntity<T>
        {
            // 创建一个字典，用于快速查找节点的子节点
            var childrenLookup = new NullableDictionary<string, List<T>>();
            foreach (var item in source.Where(item => !childrenLookup.ContainsKey(item.Id)))
            {
                childrenLookup[item.Id] = new List<T>();
            }

            // 构建树结构
            foreach (var item in source.Where(item => string.IsNullOrEmpty(item.ParentId) && childrenLookup.ContainsKey(item.ParentId)))
            {
                childrenLookup[item.ParentId].Add(item);
            }

            // 找到根节点，即没有父节点的节点
            foreach (var root in source.Where(x => Equals(x.ParentId, parent.Id)))
            {
                // 为根节点和所有子节点设置Children属性
                // 使用队列来模拟递归过程
                var queue = new Queue<T>();
                queue.Enqueue(root);

                while (queue.Count > 0)
                {
                    // 出队当前节点
                    var current = queue.Dequeue();

                    // 为当前节点设置子节点
                    if (childrenLookup.TryGetValue(current.Id, out var children))
                    {
                        current.Children = children;
                        foreach (var child in children)
                        {
                            // 如果子节点实现了ITreeParent接口，则设置其Parent属性
                            if (child is ITreeParent<T> tree)
                            {
                                tree.Parent = current;
                            }

                            // 将子节点入队以继续处理
                            queue.Enqueue(child);
                        }
                    }
                }
                yield return root;
            }
        }

        private static IEnumerable<T> TransData<T, TKey>(IEnumerable<T> source, T parent, Func<T, TKey> idSelector, Func<T, TKey?> pidSelector) where T : ITreeChildren<T> where TKey : struct
        {
            // 创建一个字典，用于快速查找节点的子节点
            var childrenLookup = new NullableDictionary<TKey, List<T>>();
            foreach (var item in source.Where(item => !childrenLookup.ContainsKey(idSelector(item))))
            {
                childrenLookup[idSelector(item)] = new List<T>();
            }

            // 构建树结构
            foreach (var item in source.Where(item => Equals(pidSelector(item), default(TKey)) && childrenLookup.ContainsKey(pidSelector(item).Value)))
            {
                childrenLookup[pidSelector(item).Value].Add(item);
            }

            // 找到根节点，即没有父节点的节点
            foreach (var root in source.Where(x => Equals(pidSelector(x), idSelector(parent))))
            {
                // 为根节点和所有子节点设置Children属性
                // 使用队列来模拟递归过程
                var queue = new Queue<T>();
                queue.Enqueue(root);

                while (queue.Count > 0)
                {
                    // 出队当前节点
                    var current = queue.Dequeue();

                    // 为当前节点设置子节点
                    if (childrenLookup.TryGetValue(idSelector(current), out var children))
                    {
                        current.Children = children;
                        foreach (var child in children)
                        {
                            // 如果子节点实现了ITreeParent接口，则设置其Parent属性
                            if (child is ITreeParent<T> tree)
                            {
                                tree.Parent = current;
                            }

                            // 将子节点入队以继续处理
                            queue.Enqueue(child);
                        }
                    }
                }
                yield return root;
            }
        }

        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="idSelector"></param>
        /// <param name="pidSelector"></param>
        /// <param name="topValue">根对象parentId的值</param>
        /// <returns></returns>
        public static List<Tree<T>> ToTreeGeneral<T, TKey>(this IEnumerable<T> source, Expression<Func<T, TKey>> idSelector, Expression<Func<T, TKey>> pidSelector, TKey topValue = default) where TKey : IComparable
        {
            if (idSelector.Body.ToString() == pidSelector.Body.ToString())
            {
                throw new ArgumentException("idSelector和pidSelector不应该为同一字段！");
            }

            var pidFunc = pidSelector.Compile();
            var idFunc = idSelector.Compile();
            source = source.Where(t => t != null);
            var temp = new List<Tree<T>>();
            foreach (var item in source.Where(item => pidFunc(item) is null || pidFunc(item).Equals(topValue)))
            {
                var parent = new Tree<T>(item);
                TransData(source, parent, idFunc, pidFunc);
                temp.Add(parent);
            }

            return temp;
        }

        private static void TransData<T, TKey>(IEnumerable<T> source, Tree<T> parent, Func<T, TKey> idSelector, Func<T, TKey> pidSelector) where TKey : IComparable
        {
            var temp = new List<Tree<T>>();
            foreach (var item in source.Where(item => pidSelector(item)?.Equals(idSelector(parent.Value)) == true))
            {
                var p = new Tree<T>(item);
                TransData(source, p, idSelector, pidSelector);
                p.Parent = parent.Value;
                temp.Add(p);
            }

            parent.Children = temp;
        }

        /// <summary>
        /// 所有子级
        /// </summary>
        public static ICollection<T> AllChildren<T>(this T tree) where T : ITreeChildren<T> => GetChildren(tree, c => c.Children);

        /// <summary>
        /// 所有子级
        /// </summary>
        public static ICollection<T> AllChildren<T>(this T tree, Func<T, IEnumerable<T>> selector) where T : ITreeChildren<T> => GetChildren(tree, selector);

        /// <summary>
        /// 所有子级
        /// </summary>
        public static ICollection<Tree<T>> AllChildren<T>(this Tree<T> tree) => GetChildren(tree, c => c.Children);

        /// <summary>
        /// 所有子级
        /// </summary>
        public static ICollection<Tree<T>> AllChildren<T>(this Tree<T> tree, Func<Tree<T>, IEnumerable<Tree<T>>> selector) => GetChildren(tree, selector);

        /// <summary>
        /// 所有父级
        /// </summary>
        public static ICollection<T> AllParent<T>(this T tree) where T : ITreeParent<T> => GetParents(tree, c => c.Parent);

        /// <summary>
        /// 所有父级
        /// </summary>
        public static ICollection<T> AllParent<T>(this T tree, Func<T, T> selector) where T : ITreeParent<T> => GetParents(tree, selector);

        /// <summary>
        /// 所有父级
        /// </summary>
        public static ICollection<Tree<T>> AllParent<T>(this Tree<T> tree, Func<Tree<T>, Tree<T>> selector) => GetParents(tree, selector);

        /// <summary>
        /// 是否是根节点
        /// </summary>
        public static bool IsRoot<T>(this ITreeParent<T> tree) where T : ITreeParent<T> => tree.Parent == null;

        /// <summary>
        /// 是否是叶子节点
        /// </summary>
        public static bool IsLeaf<T>(this ITreeChildren<T> tree) where T : ITreeChildren<T> => tree.Children?.Count == 0;

        /// <summary>
        /// 是否是根节点
        /// </summary>
        public static bool IsRoot<T>(this Tree<T> tree) => tree.Parent == null;

        /// <summary>
        /// 是否是叶子节点
        /// </summary>
        public static bool IsLeaf<T>(this Tree<T> tree) => tree.Children?.Count == 0;

        /// <summary>
        /// 深度层级
        /// </summary>
        public static int Level<T>(this ITreeParent<T> tree) where T : ITreeParent<T> => IsRoot(tree) ? 1 : Level(tree.Parent) + 1;

        /// <summary>
        /// 节点路径（UNIX路径格式，以“/”分隔）
        /// </summary>
        public static string Path<T>(this T tree) where T : ITree<T> => GetFullPath(tree, t => t.Name);

        /// <summary>
        /// 节点路径（UNIX路径格式，以“/”分隔）
        /// </summary>
        public static string Path<T>(this T tree, Func<T, string> selector) where T : ITreeParent<T> => GetFullPath(tree, selector);

        /// <summary>
        /// 节点路径
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="separator">分隔符</param>
        public static string Path<T>(this T tree, string separator) where T : ITree<T> => GetFullPath(tree, t => t.Name, separator);

        /// <summary>
        /// 节点路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tree"></param>
        /// <param name="selector">选择字段</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string Path<T>(this T tree, Func<T, string> selector, string separator) where T : ITreeParent<T> => GetFullPath(tree, selector, separator);

        /// <summary>
        /// 根节点
        /// </summary>
        public static T Root<T>(this T tree) where T : ITreeParent<T> => GetRoot(tree, t => t.Parent);

        private static string GetFullPath<T>(T c, Func<T, string> selector, string separator = "/") where T : ITreeParent<T> => c.Parent != null ? GetFullPath(c.Parent, selector, separator) + separator + selector(c) : selector(c);

        /// <summary>
        /// 根节点
        /// </summary>
        public static T GetRoot<T>(T c, Func<T, T> selector) where T : ITreeParent<T> => c.Parent != null ? GetRoot(c.Parent, selector) : c;

        /// <summary>
        /// 递归取出所有下级
        /// </summary>
        /// <param name="t"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        private static List<T> GetChildren<T>(T t, Func<T, IEnumerable<T>> selector)
        {
            return selector(t).Union(selector(t).Where(c => selector(c)?.Any() == true).SelectMany(c => GetChildren(c, selector))).ToList();
        }

        /// <summary>
        /// 递归取出所有上级
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static List<T> GetParents<T>(T t, Func<T, T> selector)
        {
            var list = new List<T>() { selector(t) };
            if (selector(t) != null)
            {
                return list.Union(GetParents(selector(t), selector)).Where(x => x != null).ToList();
            }

            list.RemoveAll(x => x == null);
            return list;
        }
    }

    public static class TreeExtensionLong
    {
        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> ToTree<T>(this IEnumerable<T> source) where T : ITreeEntity<T, long>
        {
            return source.ToTree<T, long>();
        }
    }

    public static class TreeExtensionGuid
    {
        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> ToTree<T>(this IEnumerable<T> source) where T : ITreeEntity<T, Guid>
        {
            return source.ToTree<T, Guid>();
        }
    }

    public static class TreeExtensionString
    {
        /// <summary>
        /// 平行集合转换成树形结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> ToTree<T>(this IEnumerable<T> source) where T : ITreeEntity<T>
        {
            if (source is IQueryable<T> queryable)
            {
                source = queryable.ToList();
            }

            source = source.Where(t => t != null).ToList();
            var temp = new List<T>();
            foreach (var item in source.Where(item => item.ParentId is null || item.ParentId.Equals(default)))
            {
                temp.AddRange(TreeExtensions.TransData(source, item));
            }

            return temp;
        }
    }
}