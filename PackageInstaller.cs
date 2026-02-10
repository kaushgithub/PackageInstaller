using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackageDependencyInstaller
{
    /// <summary>
    /// A utility to determine installation order for packages based on dependencies.
    /// Detects cycles and prints an installation order respecting dependencies.
    /// </summary>
    public class PackageInstaller
    {
        /// <summary>
        /// Returns a comma-separated string of packages in installable order.
        /// Throws an exception if cycles are detected.
        /// </summary>
        /// <param name="inputDependencies">Array of strings in the format "Package:Dependency"</param>
        /// <returns>Comma-separated installation order</returns>
        public string PrintDependencies(string[] inputDependencies)
        {
            if (inputDependencies == null || inputDependencies.Length < 1)
                throw new ArgumentNullException(nameof(inputDependencies));

            var dependentOn = new List<string>(); // Holds packages that are parents
            var dependencyRelation = new Dictionary<string, HashSet<string>>(); // Parent -> set of dependents
            List<string> noDependents = new List<string>(); // Packages with no dependencies

            // Build dependency graph
            foreach (var inputDependency in inputDependencies)
            {
                var parts = inputDependency.Split(':');
                if (parts.Length != 2)
                    throw new ArgumentException($"Invalid input format: {inputDependency}");

                string dependent = parts[0].Trim();
                string dependsOn = parts[1].Trim();

                if (!string.IsNullOrEmpty(dependsOn))
                {
                    // Check cycles before adding
                    CheckCycles(dependencyRelation, dependent, dependsOn);

                    // Add dependent to parent's list
                    if (!dependencyRelation.ContainsKey(dependsOn))
                        dependencyRelation[dependsOn] = new HashSet<string>();

                    dependencyRelation[dependsOn].Add(dependent);

                    // Maintain ordering in dependentOn list
                    dependentOn = AddDependencyInOrder(dependentOn, dependsOn, dependent);
                }
                else
                {
                    // No dependencies
                    noDependents.Add(dependent);
                }
            }

            // Build the final install order
            var visited = new HashSet<string>();
            var result = new StringBuilder();
            bool first = true;

            void AppendPackage(string pkg)
            {
                if (!visited.Contains(pkg))
                {
                    if (!first) result.Append(", ");
                    result.Append(pkg);
                    first = false;
                    visited.Add(pkg);
                }
            }

            // Add packages with no dependencies first
            foreach (var pkg in noDependents)
                AppendPackage(pkg);

            // Add dependent packages respecting order
            foreach (var pkg in dependentOn)
            {
                AppendPackage(pkg);

                if (dependencyRelation.ContainsKey(pkg))
                {
                    foreach (var dep in dependencyRelation[pkg])
                        AppendPackage(dep);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Adds a package to the dependentOn list in the correct order based on dependencies.
        /// </summary>
        private List<string> AddDependencyInOrder(List<string> dependentOn, string parent, string child)
        {
            if (dependentOn.Contains(parent))
                return dependentOn;

            if (dependentOn.Contains(child))
            {
                // Insert parent before child
                var newList = new List<string> { parent };
                newList.AddRange(dependentOn);
                return newList;
            }

            dependentOn.Add(parent);
            return dependentOn;
        }

        /// <summary>
        /// Detects cycles in the dependency graph.
        /// Throws ArgumentException if a cycle is found.
        /// </summary>
        private void CheckCycles(Dictionary<string, HashSet<string>> dependencyRelation, string dependent, string dependsOn)
        {
            if (!dependencyRelation.ContainsKey(dependent))
                return;

            var toCheck = new Queue<string>(dependencyRelation[dependent]);

            while (toCheck.Count > 0)
            {
                var current = toCheck.Dequeue();
                if (current == dependsOn)
                    throw new ArgumentException("Dependency graph contains a cycle");

                if (dependencyRelation.ContainsKey(current))
                {
                    foreach (var child in dependencyRelation[current])
                        toCheck.Enqueue(child);
                }
            }
        }
    }
}
