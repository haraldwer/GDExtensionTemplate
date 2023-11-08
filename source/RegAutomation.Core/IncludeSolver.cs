using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegAutomation.Core
{
    public static class IncludeSolver
    {
        public static string Solve(List<string> headerPaths, List<List<string>> headerDependencies, List<bool> headerHasTypes, List<string> headerIncludeNames)
        {
            // Step 1: Linearize all headers into a zero-based index.
            // This maintains the mapping from header filepaths to indices.
            Dictionary<string, int> keyToIndex = new Dictionary<string, int>();
            // This maintains every header's dependency, which can change during the algorithm.
            List<List<int>> indexToDependency = new List<List<int>>();
            for(int i = 0; i < headerPaths.Count; i++)
            {
                keyToIndex[headerPaths[i]] = i;
                indexToDependency.Add(new List<int>());
            }
            for(int i = 0; i < headerDependencies.Count; i++)
            {
                for(int j = 0; j < headerDependencies[i].Count; j++)
                {
                    indexToDependency[i].Add(keyToIndex[headerDependencies[i][j]]);
                }
            }
            // Step 2: Find reg class dependencies for every reg class header.
            // We update indexToDependency here to limit the scope to reg class headers.
            // Note: Worst case O(N^2) (quadratic to total header count), but should practically never happen.
            // TODO: Come up with an algorithm that has a better worst-case time complexity.
            // A list of reg class headers.
            List<int> regClassHeaderIndices = new List<int>();
            // Keeps track of visited headers during depth-first search.
            List<bool> visited = new List<bool>();
            // Keeps track of how many reg class headers include this header.
            // Not actually used for non-reg class headers, but their spots are reserved for O(1) indexing.
            List<int> regClassIncludedCount = new List<int>();
            // Step 2.1: Initialization.
            for (int i = 0; i < headerPaths.Count; i++)
            {
                if (headerHasTypes[i])
                    regClassHeaderIndices.Add(i);
                visited.Add(false);
                regClassIncludedCount.Add(0);
            }
            // Step 2.2: Recursively search for reg class headers and
            // update dependencies to only contain reg class headers.
            foreach (int index in regClassHeaderIndices)
            {
                if (!headerHasTypes[index]) continue;
                List<int> updatedDependency = new List<int>();
                RecursiveFindRegClassDependency(index, updatedDependency);
                foreach (int depIndex in updatedDependency)
                    regClassIncludedCount[depIndex]++;
                indexToDependency[index] = updatedDependency;
            }
            // Find immediate reg class headers and add them to the updatedDependency list.
            // "Immediate" means no other reg class headers between it and the source reg class header.
            void RecursiveFindRegClassDependency(int currentIndex, List<int> updatedDependency)
            {
                if (visited[currentIndex]) 
                    return;
                visited[currentIndex] = true;
                foreach(int depIndex in indexToDependency[currentIndex])
                {
                    if (headerHasTypes[depIndex])
                        updatedDependency.Add(depIndex);
                    else
                        RecursiveFindRegClassDependency(depIndex, updatedDependency);
                }
                visited[currentIndex] = false;
            }
            // Step 3: Find topological ordering of reg class headers.
            // Kahn's algorithm is used here.
            List<int> topologicalOrder = new List<int>();
            Queue<int> searchQueue = new Queue<int>();
            // Step 3.1: Find all reg class headers that aren't included by other headers.
            // Our search always starts from such headers.
            foreach (int index in regClassHeaderIndices)
            {
                if (regClassIncludedCount[index] == 0) 
                    searchQueue.Enqueue(index);
            }
            // Step 3.2: Keep removing out-edges from headers in the search queue.
            // Repeat until the queue is empty.
            while(searchQueue.TryDequeue(out int index))
            {
                topologicalOrder.Add(index);
                foreach(int depIndex in indexToDependency[index])
                {
                    regClassIncludedCount[depIndex]--;
                    if (regClassIncludedCount[depIndex] == 0)
                        searchQueue.Enqueue(depIndex);
                }
            }
            // Step 3.3: Verify if there are any cycles left. If so, output all headers that are in a cycle.
            // It is impossible to find a valid include sequence if there are cycles between reg class headers, so we throw.
            StringBuilder cycleList = new StringBuilder();
            foreach (int index in regClassHeaderIndices)
            {
                if (regClassIncludedCount[index] != 0)
                    cycleList.AppendLine(headerIncludeNames[index]); 
            }
            if(cycleList.Length > 0)
            {
                throw new Exception($"Detected circular dependency! Check the following headers:\n{cycleList}");
            }
            // Step 4: Output the include order as the reverse topological order.
            StringBuilder result = new StringBuilder();
            // We use the templated LINQ version of Reverse to iterate over topological order in reverse.
            foreach(int index in topologicalOrder.Reverse<int>())
            {
                result.Append($"#include \".generated/{headerIncludeNames[index]}.generated.h\"\n");
            }
            return result.ToString();
        }
    }
}
