using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using JobLib; // Required for JobBundles and Executor

namespace Exercise.Pages
{
    public class IndexModel : PageModel
    {
        public IDictionary<string, string> PredefinedBundles { get; set; }
        public string ExecutionResult { get; set; }

        [BindProperty]
        public string SelectedBundle { get; set; }

        [BindProperty]
        public string CustomPattern { get; set; }

        public void OnGet()
        {
            // Ensure JobBundles are loaded if they are not already (e.g. by a previous run or static constructor)
            // This is a bit of a workaround as the original Program.cs initialized this.
            // In a real web app, this might be loaded from a config file or database.
            LoadJobBundlesIfNotPresent();
            PredefinedBundles = new Dictionary<string, string>(JobBundles.BundleDict);
        }

        public IActionResult OnPostExecuteBundle()
        {
            LoadJobBundlesIfNotPresent();
            if (!string.IsNullOrEmpty(SelectedBundle) && JobBundles.BundleDict.ContainsKey(SelectedBundle))
            {
                ExecutionResult = Executor.RunJobs(SelectedBundle);
            }
            else
            {
                ExecutionResult = "Error: Selected bundle not found or invalid.";
            }
            PredefinedBundles = new Dictionary<string, string>(JobBundles.BundleDict);
            return Page();
        }

        public IActionResult OnPostExecuteCustomPattern()
        {
            LoadJobBundlesIfNotPresent();
            const string customBundleKey = "custom_temp_pattern";
            if (!string.IsNullOrWhiteSpace(CustomPattern))
            {
                JobBundles.BundleDict.Add(customBundleKey, CustomPattern);
                try
                {
                    ExecutionResult = Executor.RunJobs(customBundleKey);
                }
                finally
                {
                    JobBundles.BundleDict.Remove(customBundleKey);
                }
            }
            else
            {
                ExecutionResult = "Error: Custom pattern cannot be empty.";
            }
            PredefinedBundles = new Dictionary<string, string>(JobBundles.BundleDict);
            return Page();
        }

        // Helper to ensure JobBundles are initialized
        private void LoadJobBundlesIfNotPresent()
        {
            if (JobBundles.BundleDict.Count == 0)
            {
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
            }
        }
    }
}
