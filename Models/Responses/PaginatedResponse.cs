using System;
using System.Collections.Generic;
using System.Text;

namespace DustsSpaceLaunchTracker.Models.Responses
{
    /// <summary>
    /// Common pagination wrapper used by almost every list endpoint.
    /// </summary>
    /// <typeparam name="T">Type of item in .Results (Launch, Agency, Pad, ...)</typeparam>
    public class PaginatedResponse<T>
    {
        public int Count { get; set; }

        public string? Next { get; set; }      // full URL or null
        public string? Previous { get; set; }  // full URL or null

        public List<T> Results { get; set; } = new List<T>();
    }
}
