

namespace JobLib
{
    using System;
    using System.Linq;
    using JobLib.Struct;
    using JobLib.Util;
    
    public static  class Executor 
    {
      
        #region Main Point

        public static String RunJobs(string jobBundleNumber)
        {
             return  RunJobBundle(jobBundleNumber);
        }


        #endregion


        #region Member Method - RunJobBundle



        /// <summary>
        /// Jobs return their names
        /// </summary>
        /// <param name="jobBundleName">pass the job bundle number to starting parameter </param>
        private static String RunJobBundle(String jobBundleNumber)
        {
            if (String.IsNullOrEmpty(jobBundleNumber)) return String.Empty;

            JobSorter<Job> jobResolver = new JobSorter<Job>();
            string dependencyPatern = String.Empty;

            JobBundles.BundleDict.TryGetValue(jobBundleNumber, out dependencyPatern);
            string[] perSubDependencies = dependencyPatern.Split(';');
            char tmpPrevious = '@';
            string depPatern = String.Empty;
            var result = String.Empty;

            try
            {
                foreach (String perTaskDependency in perSubDependencies)
                {
                    depPatern = System.Text.RegularExpressions.Regex.Replace(perTaskDependency, @"\s+", "");
                    // here we detect dependend task/job was set or not that based on given task pattern 
                    // if something was configured  for ex : as A => B 
                    //for ex: this will find the depended task with name  " B " 
                    if (Char.IsLetter(depPatern.Substring(depPatern.Length - 1, 1)[0]))
                    {
                        jobResolver.Add(new Job { JobName = depPatern.Substring(0, 1)[0], Message = $"JobName : " + depPatern.Substring(0, 1)[0] },

                             new[] { new Job { JobName = depPatern.Substring(depPatern.Length - 1, 1)[0], Message = $"JobName : " + depPatern.Substring(depPatern.Length - 1, 1)[0] } }
                             );

                        tmpPrevious = depPatern.Substring(depPatern.Length - 1, 1)[0];
                    }
                    else
                    {
                        // if the task was added as a dependency should not add again .!!
                        if (tmpPrevious == depPatern.Substring(0, 1)[0])
                            continue;

                        jobResolver.Add(new Job { JobName = depPatern.Substring(0, 1)[0], Message = $"JobName : " + depPatern.Trim().Substring(0, 1)[0] });
                    }

                }

                //sorting the jobs with corresponding patterns sequence
                var sortedResult = jobResolver.Sort();
                var sorted = sortedResult.Item1;
                var cycled = sortedResult.Item2;              
                if (!cycled.Any())
                {               
                    result = String.Join(' ', sorted.ToArray<Job>().Select(j => j.Message));
                }
                else
                {
                    result = String.Format("Cycled dependencies detected: {0} ", String.Join(' ', cycled.ToArray<Job>().Select(j => j.Message)));
                }

            }
            catch (Exception ex) // beside the main duty : it do catch the self dependencies error.
            {

                result = ex.Message;
            }

         
            return result;
         }


        #endregion

    }
}
