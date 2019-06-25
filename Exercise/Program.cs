namespace Exercise
{
    using System;
    using JobLib;

    class Program
    {
        static void Main(string[] args)
        {
            // add job pattern and bundle number for preparing run
            JobBundles.BundleDict.Add("1", "a=>");
            JobBundles.BundleDict.Add("2", "a=>;b=>;c=>");
            JobBundles.BundleDict.Add("3", "a=>;b=>c;c=>");
            JobBundles.BundleDict.Add("4", @"a=>;
                                             b=>c;
                                             c=>f;                                           
                                             d=>a;
                                             e=>b;
                                             f=>");
            JobBundles.BundleDict.Add("5", @"a=>;
                                             b=>;
                                             c=>c");
            JobBundles.BundleDict.Add("6", @"a =>;
                                             b =>c;
                                             c=>f;
                                             d=>a;
                                             e =>;
                                             f =>b");

            // run all jobs 
            // it returns a string value that contains 'a job name: name format
            foreach (var bundleNumber in JobBundles.BundleDict.Keys)
            {
                Console.WriteLine($"Job Bundle Number : {bundleNumber}");
                Console.WriteLine(Executor.RunJobs(bundleNumber));
                Console.WriteLine("*************************************");

            }

            Console.ReadKey(true);
        }
    }
}
