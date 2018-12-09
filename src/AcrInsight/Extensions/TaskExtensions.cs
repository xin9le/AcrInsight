using System.Collections.Generic;
using System.Threading.Tasks;



namespace AcrInsight.Extensions
{
    /// <summary>
    /// Provides extension methods of <see cref="Task{T}"/>.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Creates a task that will complete when all of the <see cref="Task{T}"/> objects in an enumerable collection have completed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
            => Task.WhenAll(tasks);
    }
}
