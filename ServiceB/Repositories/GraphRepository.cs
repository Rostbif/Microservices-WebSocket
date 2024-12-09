using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceA.Models;

namespace ServiceB.Repositories
{
    public class GraphRepository
    {
        private readonly string _filePath = "graphs.json";
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<List<Graph>> ReadGraphsAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<Graph>();
                }

                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<List<Graph>>(json) ?? new List<Graph>();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task WriteGraphsAsync(List<Graph> graphs)
        {
            await _semaphore.WaitAsync();
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var json = JsonSerializer.Serialize(graphs, options); await File.WriteAllTextAsync(_filePath, json);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}