using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageDependencyInstaller
{
    public class PackageInstaller
    {
        public string PrintDependencies(string[] inputDependencies)
        {
            if (inputDependencies == null || inputDependencies.Count() < 1)
                throw new ArgumentNullException("inputDependencies");
            List<string> dependentOn = new List<string>(); //Holds strings which are parent
            Dictionary<string, HashSet<string>> dependencyRelation = new Dictionary<string, HashSet<string>>(); //build a dictionary of string with dependents 
            List<string> noDependents = null; //Hold strings which do not have any dependents
            foreach (string inputDependency in inputDependencies)
            {
                var inputs = inputDependency.Split(':');
                if (inputs.Count() < 2)
                    throw new ArgumentException("Input string does not have a :");
                if (inputs.Count() > 0)
                {
                    string inputDependentOn = inputs[1].Trim();
                    string inputDependent = inputs[0].Trim();
                    if (!string.IsNullOrWhiteSpace(inputDependentOn))
                    {
                        FindCycles(dependencyRelation, inputDependent, inputDependentOn);
                    }
                    if (inputs.Count() == 2 && (!string.IsNullOrWhiteSpace(inputDependentOn)))
                    {
                        if (dependencyRelation.ContainsKey(inputDependentOn))
                        {
                            HashSet<string> temp = dependencyRelation[inputDependentOn];
                            temp.Add(inputDependent);
                            dependencyRelation[inputDependentOn] = temp;
                        }
                        else
                        {
                            HashSet<string> temp = new HashSet<string>();
                            temp.Add(inputDependent);
                            dependencyRelation.Add(inputDependentOn, temp);
                        }
                        dependentOn = WalkThroughDependentOnListAndAddCurrentDependentOn(dependentOn, inputDependentOn, inputDependent);
                    }
                    else if ((inputs.Count() == 2 && string.IsNullOrWhiteSpace(inputDependentOn)))
                    {
                        if (noDependents == null)
                        {
                            noDependents = new List<string>();
                        }
                        noDependents.Add(inputDependent);
                    }
                }
            }

            //print the dependencies and return
            HashSet<string> visited = new HashSet<string>();
            StringBuilder retval = new StringBuilder();
            bool firstDependency = false;
            if (noDependents != null && noDependents.Count > 0)
            {
                foreach (string installerPackage in noDependents)
                {
                    if (!visited.Contains(installerPackage))
                    {
                        if (!firstDependency)
                        {
                            retval.Append(installerPackage);
                            firstDependency = true;
                        }
                        else
                        {
                            retval.Append( ", " + installerPackage);
                        }
                        visited.Add(installerPackage);
                    }
                }
            }
            foreach (string installerPackage in dependentOn)
            {
                if (!visited.Contains(installerPackage))
                {
                    if (!firstDependency)
                    {
                        retval.Append(installerPackage);
                        firstDependency = true;
                    }
                    else
                    {
                        retval.Append(", " + installerPackage);
                    }
                    visited.Add(installerPackage);
                }
                HashSet<string> temp = dependencyRelation[installerPackage];
                foreach (var dependent in temp)
                {
                    if (!visited.Contains(dependent))
                    {
                        if (!firstDependency)
                        {
                            retval.Append(dependent);
                            firstDependency = true;
                        }
                        else
                        {
                            retval.Append(", " +  dependent);
                        }
                        visited.Add(dependent);
                    }
                }
            }
            return retval.ToString();
        }

        //This method walks through the dependenton list and reorders the list based on dependency and adds the addDependentOn at the correct position
        private List<string> WalkThroughDependentOnListAndAddCurrentDependentOn(List<string> dependentOn, string addDependentOn, string dependent)
        {
            if (dependentOn.Contains(addDependentOn))
                return dependentOn; //if it is already there, just return
            if (dependentOn.Contains(dependent))
            {
                //reconstruct the list
                List<string> tempList = new List<string>(dependentOn.Count + 1);
                tempList.Add(addDependentOn); //add before 
                tempList.AddRange(dependentOn);
                dependentOn = tempList;
                return tempList;
            }
            dependentOn.Add(addDependentOn);  //or add at the end of list
            return dependentOn;
        }

        private void FindCycles(Dictionary<string, HashSet<string>> dependencyRelation, string dependent, string dependentOn)
        {
            if (dependencyRelation.ContainsKey(dependent))
            {
                //check for cycles
                HashSet<string> temp = new HashSet<string>(dependencyRelation[dependent].ToList());
                List<string> tempList = temp.ToList();
                if (temp.Contains(dependentOn))
                    throw new ArgumentException("Dependencies contains cycles");
                while (true)
                {
                    if (temp != null && temp.Count > 0)
                    {
                        if (temp.Contains(dependentOn))
                        {
                            throw new ArgumentException("Dependencies contains cycles");
                        }
                        tempList = temp.ToList();
                        temp.Clear();
                        foreach (var str in tempList)
                        {
                            if (dependencyRelation.ContainsKey(str))
                            {
                                foreach (var strdependecy in dependencyRelation[str].ToList())
                                {
                                    temp.Add(strdependecy);
                                }
                            }

                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }
}
